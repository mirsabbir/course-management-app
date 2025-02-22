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
            var user = await _userManager.GetUserAsync(context.Subject);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    context.IssuedClaims.Add(new Claim(JwtClaimTypes.Role, role));
                }
                context.IssuedClaims.Add(new Claim("fullName", user.FullName));
            }
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.CompletedTask;
        }
    }
}
