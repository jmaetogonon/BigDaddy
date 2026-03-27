using BigDaddy.Application.Contracts.Services;
using System.IdentityModel.Tokens.Jwt;

namespace BigDaddy.Api.Middleware;

/// <summary>
/// Runs after JWT authentication.
/// Rejects requests whose token has been invalidated (logged out).
/// </summary>
public class TokenValidationMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, IAuthService authService)
    {
        // Only check authenticated requests
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var jti = context.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

            if (!string.IsNullOrEmpty(jti))
            {
                var isInvalidated = await authService.IsTokenInvalidatedAsync(jti);

                if (isInvalidated)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(
                        """{"message":"Token has been invalidated. Please log in again."}"""
                    );
                    return;
                }
            }
        }

        await _next(context);
    }
}