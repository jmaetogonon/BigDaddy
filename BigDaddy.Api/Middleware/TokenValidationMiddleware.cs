using BigDaddy.Application.Contracts.Persistence.Auth;
using System.IdentityModel.Tokens.Jwt;

namespace BigDaddy.Api.Middleware;

/// <summary>
/// Runs after JWT authentication.
/// Rejects requests whose token has been invalidated (logged out).
/// </summary>
public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;

    public TokenValidationMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, IAuthService authService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var jti = context.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

            if (!string.IsNullOrEmpty(jti) && await authService.IsTokenInvalidatedAsync(jti))
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(
                    """{"success":false,"message":"Token has been invalidated. Please log in again."}""");
                return;
            }
        }

        await _next(context);
    }
}