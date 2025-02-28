using Authorization.Application.DTOs;
using Authorization.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [AuthorizeRolesAndScopes(roles: [], scopes: ["user.manage"])]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] CreateUserDTO request)
        {
            var id = await _userService.SendInvitationAsync(request);

            return Ok(new { userId = id });
        }

        // GET: api/users
        [AuthorizeRolesAndScopes(roles: [], scopes: ["user.manage"])]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        // GET: api/users/{id}
        [AuthorizeRolesAndScopes(roles: [], scopes: ["user.manage"])]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [AuthorizeRolesAndScopes(roles: [], scopes: ["user.manage"])]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserById(Guid id)
        {
            await _userService.DeleteUser(id);
            return Ok();
        }
    }
}
