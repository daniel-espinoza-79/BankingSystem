using DataAccess.Context;
using DataAccess.Repositories.Abstracts;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddPersistenceInfrastructure(this IServiceCollection services, string connectionString, string assemblyFullName)
        {
            services.AddDbContext<DbContext,DataAccessContext>(
                options => options.UseNpgsql(
                    connectionString,
                    b => b.MigrationsAssembly(assemblyFullName)
                )
            );
            services.AddTransient(typeof(IRepository<,>), typeof(BaseRepository<,>));
            return services;
        }
    }
}