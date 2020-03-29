using App.Data.DAL;
using App.Data.Repository;
using App.Domain.Models.Identity;
using App.Helper;
using App.Services;
using App.Services.Core;
using App.Services.Utils;
using App.Services.Localization;
using App.Web.App_Start;
using App.Web.Models.ViewModels.Hosting;
using App.Web.Utils;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.AspNetCore.NameConvention;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using App.Web.Utils.SsoConfig;
using System.IdentityModel.Tokens.Jwt;
using IdentityServer4.AccessTokenValidation;
using App.Web.Filter;
using DinkToPdf.Contracts;
using DinkToPdf;

namespace App.Web
{
    public class Startup
    {
        public class ConfirmEmailDataProtectorTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
        {
            public ConfirmEmailDataProtectorTokenProvider(IDataProtectionProvider dataProtectionProvider, IOptions<ConfirmEmailDataProtectionTokenProviderOptions> options) : base(dataProtectionProvider, options)
            {
            }
        }

        public class ConfirmEmailDataProtectionTokenProviderOptions : DataProtectionTokenProviderOptions { }


        public class PasswordResetDataProtectorTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
        {
            public PasswordResetDataProtectorTokenProvider(IDataProtectionProvider dataProtectionProvider, IOptions<PasswordResetProtectionTokenProviderOptions> options) : base(dataProtectionProvider, options)
            {
            }
        }

        public class PasswordResetProtectionTokenProviderOptions : DataProtectionTokenProviderOptions { }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddUserSecrets();
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }
        private const string EmailConfirmationTokenProviderName = "ConfirmEmail";
        private const string PasswordResetTokenProviderName = "PasswordReset";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);

            // Use a PostgresSQL Database
            var sqlConnectionString = Configuration["DataAccessPostgreSqlProvider:ConnectionString"];

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    sqlConnectionString,
                    b => b.MigrationsAssembly("App.Web")
                )
            );

            services.Configure<IdentityOptions>(options =>
            {
                options.Tokens.EmailConfirmationTokenProvider = EmailConfirmationTokenProviderName;
                options.Tokens.PasswordResetTokenProvider = PasswordResetTokenProviderName;
            });

            services.Configure<ConfirmEmailDataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(5);
            });

            services.Configure<PasswordResetProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(5);
            });

            services.Configure<HostConfiguration>(HostConfiguration =>
            {
                HostConfiguration.Name = Configuration["host:name"];
                HostConfiguration.Protocol = Configuration["host:protocol"];
            });

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
                config.Cookies.ApplicationCookie.LoginPath = "/Account/Login";
            })
                .AddEntityFrameworkStores<ApplicationDbContext, string>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<ConfirmEmailDataProtectorTokenProvider<ApplicationUser>>(EmailConfirmationTokenProviderName)
                .AddTokenProvider<PasswordResetDataProtectorTokenProvider<ApplicationUser>>(PasswordResetTokenProviderName);

            services.AddApplicationInsightsTelemetry(Configuration);

            services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = true;
            });

            services.AddSession();

            services.AddScoped<LogFilter>();

            // Add framework services.
            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.MaxDepth = 1;
                });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var dtTablesOptions = new DataTables.AspNet.AspNetCore.Options(10, true, true, true, new CamelCaseRequestNameConvention(),
               new CamelCaseResponseNameConvention());

            services.RegisterDataTables(dtTablesOptions);

            MapperConfig.Map(services);

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<LogFilter>();

            ServicesRegistrar.Register(services);

            HelperRegistrar.Register(services);

            services.AddSingleton<ISmtpOptionsService, SmtpOptionsService>(x => new SmtpOptionsService()
            {
                User = "dellytesting@gmail.com",
                Password = "pjkxbqiutjuxyvie",
                Server = "smtp.gmail.com",
                Port = 465,
                UseSsl = true,
                FromEmail = "dellytesting@gmail.com",
                FromName = "dellytesting@gmail.com",
                PrefferedEncoding = null,
                RequiresAunthetication = true
            });

            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            services.AddIdentityServer()
                  .AddTemporarySigningCredential()
                  .AddInMemoryClients(IdSrvConfig.GetClients())
                  .AddInMemoryScopes(IdSrvConfig.GetScopes())
                  .AddAspNetIdentity<ApplicationUser>()
                  .AddProfileService<IdentityWithAdditionalClaimsProfileService>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.AllowAnyOrigin());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseApplicationInsightsRequestTelemetry();
            app.UseApplicationInsightsExceptionTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            app.UseIdentity();

            app.UseIdentityServer();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            var SSOAuthority = "http://localhost:50447";
            var identityServerValidationOptions = new IdentityServerAuthenticationOptions
            {
                Authority = SSOAuthority,
                ScopeName = "client",
                ScopeSecret = "secret",
                AutomaticAuthenticate = true,
                RequireHttpsMetadata = false,
                SupportedTokens = SupportedTokens.Both,
                // required if you want to return a 403 and not a 401 for forbidden responses
                AutomaticChallenge = true
            };

            app.UseIdentityServerAuthentication(identityServerValidationOptions);

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
