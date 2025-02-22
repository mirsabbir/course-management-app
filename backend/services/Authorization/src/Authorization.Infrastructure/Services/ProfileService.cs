using Authorization.Domain;
using Duende.IdentityModel;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Authorization.Infrastructure.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<User> _userManager;

        public ProfileService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            // Extract the email claim from the ClaimsPrincipal (context.Subject)
            var email = context.Subject.FindFirstValue("sub");

            if (!string.IsNullOrEmpty(email))
            {
                // Retrieve the user by email using UserManager
                var user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    // Add roles to issued claims
                    foreach (var role in roles)
                    {
                        context.IssuedClaims.Add(new Claim(JwtClaimTypes.Role, role));
                    }

                    // Add other user-specific claims
                    context.IssuedClaims.Add(new Claim("fullName", user.FullName));
                    context.IssuedClaims.Add(new Claim("userId", user.Id));
                }
            }
            else
            {
                // Handle the case where email claim is not available
                context.IssuedClaims.Add(new Claim("error", "User email not found"));
            }
        }


        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.CompletedTask;
        }
    }
}
