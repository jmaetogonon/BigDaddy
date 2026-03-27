using BigDaddy.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BigDaddy.Api.Controllers.Users;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    // Only users whose token carries the "users.userlist.view" permission
    [HttpGet]
    [HasPermission("users.userlist.view")]
    public IActionResult GetUsers()
    {
        return Ok("List of users");
    }

    // Only Managers or higher
    [HttpPost]
    [Authorize(Policy = "Manager")]
    public IActionResult CreateUser()
    {
        return Ok("User created");
    }

    // Only SystemAdministrators
    [HttpDelete("{id}")]
    [Authorize(Policy = "SystemAdministrator")]
    public IActionResult DeleteUser(Guid id)
    {
        return Ok($"User {id} deleted");
    }
}
