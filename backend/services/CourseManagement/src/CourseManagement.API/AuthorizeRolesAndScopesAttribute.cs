using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Authorization.API
{
    public class AuthorizeRolesAndScopesAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _roles;
        private readonly string[] _scopes;

        public AuthorizeRolesAndScopesAttribute(string[] roles = null, string[] scopes = null)
        {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
            _roles = roles ?? Array.Empty<string>();
            _scopes = scopes ?? Array.Empty<string>();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Check if the user is authenticated
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Check roles
            if (_roles.Any() && !_roles.Any(role => user.IsInRole(role)))
            {
                context.Result = new ForbidResult();
                return;
            }

            // Check scopes
            if (_scopes.Any())
            {
                var scopeClaims = user.FindAll("scope").Select(c => c.Value).ToList();
                var allScopes = scopeClaims.SelectMany(s => s.Split(' ')).ToHashSet(); // Properly split space-separated scopes

                if (!_scopes.All(scope => allScopes.Contains(scope)))
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
        }
    }

}
