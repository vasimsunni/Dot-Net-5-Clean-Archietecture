using DotNetFive.Core.DataModel;
using DotNetFive.Core.Repository.Class;
using DotNetFive.Core.Repository.Interface;
using DotNetFive.Core.Repository.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetFive.Infrastructure.Configuration.DataExtention
{
    public static class RepositoryExtention
    {
        public static IServiceCollection RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRepository<Administrator>, Repository<Administrator>>();
            services.AddScoped<IRepository<FileUpload>, Repository<FileUpload>>();
            services.AddScoped<IRepository<Bin>, Repository<Bin>>();

            return services;
        }
    }
}
