
using DotNetFive.Infrastructure.Configuration.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetFive.Infrastructure.Configuration.DataExtention
{
    public static class CORSConfiguration
    {
        public static IServiceCollection ConfigureCORS(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                string corsUrls = configuration.AppSettings().CORSEnabledURL;
                string[] Orgins = corsUrls.Split(",");

                options.AddPolicy("CorsPolicy", p => p.WithOrigins(Orgins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            return services;
        }

        public static IApplicationBuilder EnableCORS(this IApplicationBuilder app)
        {
            app.UseCors("CorsPolicy");

            return app;
        }
    }
}
