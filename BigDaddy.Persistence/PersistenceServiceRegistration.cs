using BigDaddy.Application.Contracts.Repositories;
using BigDaddy.Persistence.Data;
using BigDaddy.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BigDaddy.Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
             options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        //services.AddDbContext<AppDbContext>(options =>
        //      options.UseSqlServer(
        //          configuration.GetConnectionString("DefaultConnection"),
        //          sql => sql.EnableRetryOnFailure(
        //              maxRetryCount: 3,
        //              maxRetryDelay: TimeSpan.FromSeconds(5),
        //              errorNumbersToAdd: null)
        //      ));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}