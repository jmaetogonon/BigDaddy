using BigDaddy.Api.Authorization;
using BigDaddy.Application.Abstractions;
using BigDaddy.Application.Common;
using BigDaddy.Application.Features.Users.Commands.ActivateUser;
using BigDaddy.Application.Features.Users.Commands.AssignRoles;
using BigDaddy.Application.Features.Users.Commands.AssignTeams;
using BigDaddy.Application.Features.Users.Commands.ChangePassword;
using BigDaddy.Application.Features.Users.Commands.CreateUser;
using BigDaddy.Application.Features.Users.Commands.DeactivateUser;
using BigDaddy.Application.Features.Users.Commands.DeleteUser;
using BigDaddy.Application.Features.Users.Commands.LockUser;
using BigDaddy.Application.Features.Users.Commands.ResetPassword;
using BigDaddy.Application.Features.Users.Commands.UnlockUser;
using BigDaddy.Application.Features.Users.Commands.UpdateUser;
using BigDaddy.Application.Features.Users.DTOs;
using BigDaddy.Application.Features.Users.Queries.GetUserById;
using BigDaddy.Application.Features.Users.Queries.GetUsers;
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
    private readonly Dispatcher _dispatcher;

    public UsersController(Dispatcher dispatcher) => _dispatcher = dispatcher;

    // ── QUERIES ────────────────────────────────────────────────────────────────

    // GET /api/users?search=...&isActive=true&page=1&pageSize=10
    [HttpGet]
    [HasPermission("users.list.view")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] GetUsersQuery query, CancellationToken ct)
    {
        var result = await _dispatcher.QueryAsync(query, ct);
        return Ok(ApiResponse<PagedResult<UserListItemDto>>.Ok(result));
    }

    // GET /api/users/me
    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken ct)
    {
        var user = await _dispatcher.QueryAsync(
            new GetUserByIdQuery { Id = CurrentUserId() }, ct);

        if (user is null) throw new NotFoundException("User", CurrentUserId());

        return Ok(ApiResponse<UserDetailDto>.Ok(user));
    }

    // GET /api/users/{id}
    [HttpGet("{id:int}")]
    [HasPermission("users.detail.view")]
    public async Task<IActionResult> GetUser(int id, CancellationToken ct)
    {
        var user = await _dispatcher.QueryAsync(new GetUserByIdQuery { Id = id }, ct);

        if (user is null) throw new NotFoundException("User", id);

        return Ok(ApiResponse<UserDetailDto>.Ok(user));
    }

    // ── COMMANDS ───────────────────────────────────────────────────────────────

    // POST /api/users
    [HttpPost]
    [HasPermission("users.create")]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserCommand command, CancellationToken ct)
    {
        var created = await _dispatcher.CommandAsync<UserDetailDto>(command, ct);
        return CreatedAtAction(
            nameof(GetUser),
            new { id = created.Id },
            ApiResponse<UserDetailDto>.Ok(created, "User created successfully."));
    }

    // PUT /api/users/{id}
    [HttpPut("{id:int}")]
    [HasPermission("users.edit")]
    public async Task<IActionResult> UpdateUser(
        int id, [FromBody] UpdateUserCommand command, CancellationToken ct)
    {
        command.Id = id;   // inject route value into command
        var updated = await _dispatcher.CommandAsync<UserDetailDto>(command, ct);
        return Ok(ApiResponse<UserDetailDto>.Ok(updated, "User updated successfully."));
    }

    // DELETE /api/users/{id}
    [HttpDelete("{id:int}")]
    [HasPermission("users.delete")]
    public async Task<IActionResult> DeleteUser(int id, CancellationToken ct)
    {
        if (id == CurrentUserId())
            return BadRequest(ApiResponse.Fail("You cannot delete your own account."));

        await _dispatcher.CommandAsync(new DeleteUserCommand { Id = id }, ct);
        return NoContent();
    }

    // PATCH /api/users/{id}/activate
    [HttpPatch("{id:int}/activate")]
    [HasPermission("users.activate")]
    public async Task<IActionResult> Activate(int id, CancellationToken ct)
    {
        await _dispatcher.CommandAsync(new ActivateUserCommand { Id = id }, ct);
        return Ok(ApiResponse.Ok("User activated."));
    }

    // PATCH /api/users/{id}/deactivate
    [HttpPatch("{id:int}/deactivate")]
    [HasPermission("users.deactivate")]
    public async Task<IActionResult> Deactivate(int id, CancellationToken ct)
    {
        if (id == CurrentUserId())
            return BadRequest(ApiResponse.Fail("You cannot deactivate your own account."));

        await _dispatcher.CommandAsync(new DeactivateUserCommand { Id = id }, ct);
        return Ok(ApiResponse.Ok("User deactivated."));
    }

    // PATCH /api/users/{id}/lock
    [HttpPatch("{id:int}/lock")]
    [HasPermission("users.lock")]
    public async Task<IActionResult> Lock(int id, CancellationToken ct)
    {
        await _dispatcher.CommandAsync(new LockUserCommand { Id = id }, ct);
        return Ok(ApiResponse.Ok("User locked."));
    }

    // PATCH /api/users/{id}/unlock
    [HttpPatch("{id:int}/unlock")]
    [HasPermission("users.unlock")]
    public async Task<IActionResult> Unlock(int id, CancellationToken ct)
    {
        await _dispatcher.CommandAsync(new UnlockUserCommand { Id = id }, ct);
        return Ok(ApiResponse.Ok("User unlocked."));
    }

    // PATCH /api/users/me/change-password
    [HttpPatch("me/change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordCommand command, CancellationToken ct)
    {
        command.UserId = CurrentUserId();  // inject from JWT
        await _dispatcher.CommandAsync(command, ct);
        return Ok(ApiResponse.Ok("Password changed successfully."));
    }

    // PATCH /api/users/{id}/reset-password
    [HttpPatch("{id:int}/reset-password")]
    [HasPermission("users.reset-password")]
    public async Task<IActionResult> ResetPassword(
        int id, [FromBody] ResetPasswordCommand command, CancellationToken ct)
    {
        command.UserId = id;   // inject route value
        await _dispatcher.CommandAsync(command, ct);
        return Ok(ApiResponse.Ok("Password reset successfully."));
    }

    // POST /api/users/{id}/roles
    [HttpPost("{id:int}/roles")]
    [HasPermission("users.assign-roles")]
    public async Task<IActionResult> AssignRoles(
        int id, [FromBody] List<int> roleIds, CancellationToken ct)
    {
        await _dispatcher.CommandAsync(
            new AssignRolesCommand { UserId = id, RoleIds = roleIds }, ct);
        return Ok(ApiResponse.Ok("Roles assigned successfully."));
    }

    // POST /api/users/{id}/teams
    [HttpPost("{id:int}/teams")]
    [HasPermission("users.assign-teams")]
    public async Task<IActionResult> AssignTeams(
        int id, [FromBody] List<int> teamIds, CancellationToken ct)
    {
        await _dispatcher.CommandAsync(
            new AssignTeamsCommand { UserId = id, TeamIds = teamIds }, ct);
        return Ok(ApiResponse.Ok("Teams assigned successfully."));
    }

    // ── HELPER ─────────────────────────────────────────────────────────────────
    private int CurrentUserId()
    {
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        return int.TryParse(sub, out var id)
            ? id
            : throw new ForbiddenException("Invalid token.");
    }
}
