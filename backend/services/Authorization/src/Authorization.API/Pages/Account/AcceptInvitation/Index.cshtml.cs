using Authorization.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Authorization.API.Pages.Account.AcceptInvitation
{
    public class IndexModel(IUserService userService, IConfiguration configuration) : PageModel
    {
        private readonly IUserService _userService = userService;
        private readonly IConfiguration _configuration = configuration;

        [BindProperty]
        public InputModel Input { get; set; }

        public async Task<IActionResult> OnGetAsync(string token)
        {
            var invitation = await _userService.GetValidInvitationAsync(token);
            if (invitation == null)
            {
                return BadRequest("Invalid or expired invitation.");
            }

            Input = new InputModel
            {
                Token = token,
                Email = invitation.Email
            };

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid) return Page();

            try
            {
                await _userService.CompleteRegistrationAsync(Input.Token, Input.Password);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Invalid invitation or user already exists.");
                return Page();
            }

            return Redirect(_configuration["CourseManagementAppHostUrl"] ?? "http://localhost:3000");
        }
    }
}
