using Microsoft.Extensions.DependencyInjection;

namespace App.Helper
{
    public static class HelperRegistrar
    {
        public static void Register(IServiceCollection services)
        {
            services.AddScoped<ViewRender>();
            services.AddScoped<ConfigHelper>();
            services.AddScoped<CommonHelper>();
            services.AddScoped<FileHelper>();
            services.AddScoped<IUserHelper, UserHelper>();
            services.AddScoped<MailingHelper>();
            services.AddScoped<ExcelHelper>();
            services.AddScoped<NotifHelper>();
        }
    }
}
