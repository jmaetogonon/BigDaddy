using BigDaddy.Application.Contracts.Services;
using BigDaddy.Application.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BigDaddy.Api.Controllers.Auth;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST /api/auth/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    // POST /api/auth/logout
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        // Get token expiry from claim
        var expClaim = User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;
        var expiry = DateTime.UtcNow.AddHours(1); // fallback

        if (expClaim is not null && long.TryParse(expClaim, out var expUnix))
            expiry = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;

        if (jti is null || sub is null)
            return Unauthorized(new { message = "Invalid token claims." });

        if (!int.TryParse(sub, out var userId))
            return Unauthorized(new { message = "Invalid user identifier." });

        await _authService.LogoutAsync(jti, userId, expiry);

        return Ok(new { message = "Logged out successfully." });
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
