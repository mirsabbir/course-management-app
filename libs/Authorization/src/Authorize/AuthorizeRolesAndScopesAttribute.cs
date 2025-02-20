using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Authorize
{
    public class AuthorizeRolesAndScopesAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _roles;
        private readonly string[] _scopes;

        public AuthorizeRolesAndScopesAttribute(string[] roles = null, string[] scopes = null)
        {
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
                var scopeClaim = user.FindFirst("scope")?.Value;
                if (scopeClaim == null || !_scopes.All(scope => scopeClaim.Split(' ').Contains(scope)))
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
        }
    }
}
