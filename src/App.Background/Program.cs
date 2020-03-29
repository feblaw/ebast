using App.Data.DAL;
using App.Data.Repository;
using App.Domain.Models.Identity;
using App.Helper;
using App.Services;
using App.Services.Core.Interfaces;
using App.Services.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.IO;


namespace App.Background
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new Program();
            Console.Read();
        }

        private readonly IServiceProvider serviceProvider;
        public IConfigurationRoot Configuration { get; private set; }

        public Program()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var Hosting = Configuration["Host"];
            Job jb = new Job(serviceProvider, userManager, Hosting);
            jb.Run();
            //jb.Testing("danang.mangesti@sangkuriang.co.id");
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            var sqlConnectionString = Configuration["ConnectionString"];

            // Register EntityFramework 7

            services.AddIdentity<ApplicationUser, ApplicationRole>(config =>
            {
                config.User.RequireUniqueEmail = true;
                config.Password = new PasswordOptions
                {
                    RequireDigit = false,
                    RequireNonAlphanumeric = false,
                    RequireLowercase = false,
                    RequireUppercase = false,
                    RequiredLength = 8,
                };
            })
           .AddEntityFrameworkStores<ApplicationDbContext, string>()
           .AddDefaultTokenProviders();

            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                sqlConnectionString
            ));

            // Register UserManager & RoleManager
            services.AddIdentity<ApplicationUser, ApplicationRole>()
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();

            // UserManager & RoleManager require logging and HttpContext dependencies
            services.AddLogging();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            ServicesRegistrar.Register(services);
            HelperRegistrar.Register(services);
        }

    }
}
