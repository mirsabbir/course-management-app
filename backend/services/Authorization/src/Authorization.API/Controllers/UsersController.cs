using Authorization.Application.DTOs;
using Authorization.Application.Interfaces;
using Authorization.Domain;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Writers;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Authorization.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

       // [AuthorizeRolesAndScopes(roles: [], scopes: ["user.manage"])]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] CreateUserDTO request)
        {
            var id = await _userService.SendInvitationAsync(request);

            return Ok(new { userId = id });
        }

        // GET: api/users
        //[AuthorizeRolesAndScopes(roles: [], scopes: ["user.manage"])]
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
    }
}
