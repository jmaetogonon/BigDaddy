using BigDaddy.Application.Contracts.Identity;
using BigDaddy.Application.Contracts.Persistence.Auth;
using BigDaddy.Application.Contracts.Persistence.Users;
using BigDaddy.Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BigDaddy.Identity.Extensions;

public static class IdentityExtensions
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ITokenService, TokenService>();

        var section = configuration.GetSection("JwtSettings");
        var secretKey = section["SecretKey"]!;

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = section["Issuer"],
                ValidateAudience = true,
                ValidAudience = section["Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnChallenge = ctx =>
                {
                    ctx.HandleResponse();
                    ctx.Response.StatusCode = 401;
                    ctx.Response.ContentType = "application/json";
                    return ctx.Response.WriteAsync(
                        """{"success":false,"message":"Unauthorized. Please log in."}""");
                },
                OnForbidden = ctx =>
                {
                    ctx.Response.StatusCode = 403;
                    ctx.Response.ContentType = "application/json";
                    return ctx.Response.WriteAsync(
                        """{"success":false,"message":"Forbidden. Insufficient permissions."}""");
                }
            };
        });

        return services;
    }
}