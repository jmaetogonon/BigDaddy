using BigDaddy.Application.Abstractions;
using BigDaddy.Application.Common;
using BigDaddy.Application.Features.Auth.Commands.Login;
using BigDaddy.Application.Features.Auth.Commands.Logout;
using BigDaddy.Application.Features.Auth.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BigDaddy.Api.Controllers.Auth;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly Dispatcher _dispatcher;

    public AuthController(Dispatcher dispatcher) => _dispatcher = dispatcher;

    // POST /api/auth/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command,
        CancellationToken ct)
    {
        var result = await _dispatcher.CommandAsync<LoginResponseDto>(command, ct);
        return Ok(ApiResponse<LoginResponseDto>.Ok(result));
    }

    // POST /api/auth/logout
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(sub, out var userId) || jti is null)
            return Unauthorized(ApiResponse.Fail("Invalid token claims."));

        var expClaim = User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;
        var expiry = long.TryParse(expClaim, out var exp)
            ? DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime
            : DateTime.UtcNow.AddHours(1);

        await _dispatcher.CommandAsync(new LogoutCommand
        {
            Jti = jti,
            UserId = userId,
            TokenExpiry = expiry
        }, ct);

        return Ok(ApiResponse.Ok("Logged out successfully."));
    }
}
