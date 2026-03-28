using BigDaddy.Application.Contracts.Persistence.Auth;
using BigDaddy.Application.Contracts.Persistence.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BigDaddy.Application;


public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>(); 

        return services;
    }
}