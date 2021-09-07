using DotNetFive.Core.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetFive.Infrastructure.Configuration.DataExtention
{
    public static class ServiceExtention
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<ICachingService, CachingService>();
            //services.AddScoped<ILoginService, LoginService>();
            //services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IUserResolverService, UserResolverService>();
            //services.AddScoped<IAdministratorService, AdministratorService>();
            //services.AddScoped<IFileUploadService, FileUploadService>();

            return services;
        }
    }
}
