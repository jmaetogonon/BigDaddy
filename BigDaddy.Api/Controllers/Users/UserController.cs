using BigDaddy.Api.Authorization;
using BigDaddy.Application.Common;
using BigDaddy.Application.Contracts.Persistence.Users;
using BigDaddy.Application.DTOs.Users;
using BigDaddy.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BigDaddy.Api.Controllers.Users;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService) => _userService = userService;

    // GET /api/users
    [HttpGet]
    [HasPermission("users.list.view")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] UserQueryDto query, CancellationToken ct)
    {
        var result = await _userService.GetUsersAsync(query, ct);
        return Ok(ApiResponse<PagedResult<UserListItemDto>>.Ok(result));
    }

    // GET /api/users/me
    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        var user = await _userService.GetUserByIdAsync(userId, ct);

        if (user is null) throw new NotFoundException("User", userId);

        return Ok(ApiResponse<UserDetailDto>.Ok(user));
    }

    // GET /api/users/{id}
    [HttpGet("{id:guid}")]
    [HasPermission("users.detail.view")]
    public async Task<IActionResult> GetUser(int id, CancellationToken ct)
    {
        var user = await _userService.GetUserByIdAsync(id, ct);

        if (user is null) throw new NotFoundException("User", id);

        return Ok(ApiResponse<UserDetailDto>.Ok(user));
    }

    // POST /api/users
    [HttpPost]
    [HasPermission("users.create")]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserDto dto, CancellationToken ct)
    {
        var created = await _userService.CreateUserAsync(dto, ct);
        return CreatedAtAction(
            nameof(GetUser),
            new { id = created.Id },
            ApiResponse<UserDetailDto>.Ok(created, "User created successfully."));
    }

    // PUT /api/users/{id}
    [HttpPut("{id:guid}")]
    [HasPermission("users.edit")]
    public async Task<IActionResult> UpdateUser(
        int id, [FromBody] UpdateUserDto dto, CancellationToken ct)
    {
        var updated = await _userService.UpdateUserAsync(id, dto, ct);
        return Ok(ApiResponse<UserDetailDto>.Ok(updated, "User updated successfully."));
    }

    // DELETE /api/users/{id}
    [HttpDelete("{id:guid}")]
    [HasPermission("users.delete")]
    public async Task<IActionResult> DeleteUser(int id, CancellationToken ct)
    {
        if (id == GetCurrentUserId())
            return BadRequest(ApiResponse.Fail("You cannot delete your own account."));

        await _userService.DeleteUserAsync(id, ct);
        return NoContent();
    }

    // PATCH /api/users/{id}/activate
    [HttpPatch("{id:guid}/activate")]
    [HasPermission("users.activate")]
    public async Task<IActionResult> Activate(int id, CancellationToken ct)
    {
        await _userService.ActivateUserAsync(id, ct);
        return Ok(ApiResponse.Ok("User activated."));
    }

    // PATCH /api/users/{id}/deactivate
    [HttpPatch("{id:guid}/deactivate")]
    [HasPermission("users.deactivate")]
    public async Task<IActionResult> Deactivate(int id, CancellationToken ct)
    {
        if (id == GetCurrentUserId())
            return BadRequest(ApiResponse.Fail("You cannot deactivate your own account."));

        await _userService.DeactivateUserAsync(id, ct);
        return Ok(ApiResponse.Ok("User deactivated."));
    }

    // PATCH /api/users/{id}/lock
    [HttpPatch("{id:guid}/lock")]
    [HasPermission("users.lock")]
    public async Task<IActionResult> Lock(int id, CancellationToken ct)
    {
        await _userService.LockUserAsync(id, ct);
        return Ok(ApiResponse.Ok("User locked."));
    }

    // PATCH /api/users/{id}/unlock
    [HttpPatch("{id:guid}/unlock")]
    [HasPermission("users.unlock")]
    public async Task<IActionResult> Unlock(int id, CancellationToken ct)
    {
        await _userService.UnlockUserAsync(id, ct);
        return Ok(ApiResponse.Ok("User unlocked."));
    }

    // PATCH /api/users/me/change-password
    [HttpPatch("me/change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordDto dto, CancellationToken ct)
    {
        await _userService.ChangePasswordAsync(GetCurrentUserId(), dto, ct);
        return Ok(ApiResponse.Ok("Password changed successfully."));
    }

    // PATCH /api/users/{id}/reset-password
    [HttpPatch("{id:guid}/reset-password")]
    [HasPermission("users.reset-password")]
    public async Task<IActionResult> ResetPassword(
        int id, [FromBody] ResetPasswordDto dto, CancellationToken ct)
    {
        await _userService.ResetPasswordAsync(id, dto, ct);
        return Ok(ApiResponse.Ok("Password reset successfully."));
    }

    // POST /api/users/{id}/roles
    [HttpPost("{id:guid}/roles")]
    [HasPermission("users.assign-roles")]
    public async Task<IActionResult> AssignRoles(
        int id, [FromBody] List<int> roleIds, CancellationToken ct)
    {
        await _userService.AssignRolesAsync(id, roleIds, ct);
        return Ok(ApiResponse.Ok("Roles assigned successfully."));
    }

    // POST /api/users/{id}/teams
    [HttpPost("{id:guid}/teams")]
    [HasPermission("users.assign-teams")]
    public async Task<IActionResult> AssignTeams(
        int id, [FromBody] List<int> teamIds, CancellationToken ct)
    {
        await _userService.AssignTeamsAsync(id, teamIds, ct);
        return Ok(ApiResponse.Ok("Teams assigned successfully."));
    }

    private int GetCurrentUserId()
    {
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
               ?? throw new ForbiddenException("Invalid token.");

        return int.TryParse(sub, out var id)
            ? id
            : throw new ForbiddenException("Invalid token.");
    }
}
