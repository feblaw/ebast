using App.Data.DAL;
using App.Data.Repository;
using App.Helper;
using App.Services.Core;
using FluentScheduler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Diagnostics;
using System.IO;
using App.Services.Core.Interfaces;

namespace App.EmailSender
{
    public class Startup
    {
        private static string environment;
        private const string defaultEnvironment = "Development";

        public Startup()
        {
            environment = Environment.GetEnvironmentVariable("App.EmailSender_Environment");

            if (string.IsNullOrWhiteSpace(environment))
            {
                environment = defaultEnvironment;
            }

            IServiceCollection serviceCollection = new ServiceCollection();

            ConfigureServices(serviceCollection);

            var interval = GetInterval(serviceCollection);
            var job = CreateJob(serviceCollection);

            StartEmailSender(job, interval);

            JobManager.AddJob(
                () => interval = CheckConfig(interval, serviceCollection),
                (s) => s.NonReentrant()
                    .WithName("config.checker")
                    .ToRunNow()
                    .AndEvery(10)
                    .Seconds());
        }

        private int GetInterval(IServiceCollection serviceCollection)
        {
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            var configHelper = serviceProvider.GetService<ConfigHelper>();
            var interval = configHelper.GetConfigAsInt("email.scheduler.interval");
            return interval;
        }

        private int CheckConfig(int prevInterval, IServiceCollection serviceCollection)
        {
            var interval = GetInterval(serviceCollection);
            Console.WriteLine($"Current Interval = {prevInterval}, New Interval = {interval}");

            if (prevInterval != interval)
            {
                JobManager.RemoveJob("email.sender");
                var job = CreateJob(serviceCollection);
                StartEmailSender(job, interval);
            }

            return interval;
        }

        private EmailSenderJob CreateJob(IServiceCollection serviceCollection)
        {
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            var emailArchieveService = serviceProvider.GetService<IEmailArchieveService>();
            var mailingHelper = serviceProvider.GetService<MailingHelper>();
            var configHelper = serviceProvider.GetService<ConfigHelper>();
            var commonHelper = serviceProvider.GetService<CommonHelper>();
            var job = new EmailSenderJob(emailArchieveService,
                                mailingHelper,
                                configHelper,
                                commonHelper);

            return job;
        }

        private void StartEmailSender(EmailSenderJob job, int interval)
        {
            JobManager.AddJob(
                () => job.Execute(),
                (s) => s.NonReentrant()
                    .WithName("email.sender")
                    .ToRunNow()
                    .AndEvery(interval)
                    .Seconds());
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var applicationEnvironment = PlatformServices.Default.Application;
            services.AddSingleton(applicationEnvironment);

            var appDirectory = Directory.GetCurrentDirectory();

            var environment = new HostingEnvironment
            {
                WebRootFileProvider = new PhysicalFileProvider(appDirectory),
                ApplicationName = "App.EmailSender"
            };

            services.AddSingleton<IHostingEnvironment>(environment);

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.FileProviders.Clear();
                options.FileProviders.Add(new PhysicalFileProvider(appDirectory));
            });

            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();

            var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
            services.AddSingleton<DiagnosticSource>(diagnosticSource);

            ILoggerFactory loggerFactory = new LoggerFactory()
                .AddDebug();

            services.AddSingleton(loggerFactory);
            services.AddLogging();
            services.AddMvc();

            IConfigurationRoot configuration = GetConfiguration();
            services.AddSingleton<IConfiguration>(configuration);

            var sqlConnectionString = configuration["DataAccessPostgreSqlProvider:ConnectionString"];

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    sqlConnectionString,
                    b => b.MigrationsAssembly("App.Web")
                )
            );


            services.AddSingleton(typeof(IRepository<>), typeof(Repository<>))
                .AddSingleton<IWebSettingService, WebSettingService>()
                .AddSingleton<IEmailArchieveService, EmailArchieveService>()
                .AddSingleton<ConfigHelper>()
                .AddSingleton<CommonHelper>()
                .AddSingleton<ViewRender>()
                .AddSingleton<MailingHelper>()
                .BuildServiceProvider();
        }

        private static IConfigurationRoot GetConfiguration()
        {
            IConfigurationBuilder configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environment.ToLowerInvariant()}.json", optional: true)
                .AddEnvironmentVariables("App.EmailSender_");

            if (string.Compare(environment, defaultEnvironment, true) == 0)
            {
                configuration.AddUserSecrets();
            }

            return configuration.Build();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
        }
    }
}
