using Authorization.Application.DTOs;
using Authorization.Application.Interfaces;
using Authorization.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [AuthorizeRolesAndScopes(["Staff"], [])]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] CreateUserDTO request)
        {
            await _userService.SendInvitationAsync(request.Email);

            return Ok();
        }
    }
}
