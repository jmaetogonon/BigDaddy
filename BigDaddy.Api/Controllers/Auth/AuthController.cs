using BigDaddy.Application.Common;
using BigDaddy.Application.Contracts.Persistence.Auth;
using BigDaddy.Application.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BigDaddy.Api.Controllers.Auth;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    // POST /api/auth/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.Fail("Validation failed.",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

        var result = await _authService.LoginAsync(request, ct);
        return Ok(ApiResponse<LoginResponseDto>.Ok(result));
    }

    // POST /api/auth/logout
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (!int.TryParse(sub, out var userId) || jti is null)
            return Unauthorized(ApiResponse.Fail("Invalid token claims."));

        var expClaim = User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;
        var expiry = long.TryParse(expClaim, out var exp)
            ? DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime
            : DateTime.UtcNow.AddHours(1);

        await _authService.LogoutAsync(jti, userId, expiry, ct);

        return Ok(ApiResponse.Ok("Logged out successfully."));
    }

    // GET /api/auth/me  — example of a protected endpoint
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value });
        return Ok(claims);
    }
}
