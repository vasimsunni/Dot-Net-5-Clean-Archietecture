using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.UI.Services;
using DotNetFive.Infrastructure.Configuration.Application;

namespace DotNetFive.Infrastructure.Configuration.DataExtention
{
    public static class EmailSenderConfiguration
    {
        public static IServiceCollection ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddTransient<IEmailSender, EmailSender>(i =>
            //      new EmailSender(
            //       configuration.AppSettings().EmailHost,
            //       configuration.AppSettings().EmailPort,
            //       configuration.AppSettings().EmailEnableSSL,
            //       configuration.AppSettings().EmailUserName,
            //       configuration.AppSettings().EmailPassword
            //     )
            //);

            return services;
        }
    }
}
