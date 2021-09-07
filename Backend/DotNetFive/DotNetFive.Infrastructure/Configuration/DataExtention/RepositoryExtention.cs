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
            services.AddScoped<IBaseRepository<Administrator>, BaseRepository<Administrator>>();
            services.AddScoped<IBaseRepository<FileUpload>, BaseRepository<FileUpload>>();
            services.AddScoped<IBaseRepository<Bin>, BaseRepository<Bin>>();

            return services;
        }
    }
}
