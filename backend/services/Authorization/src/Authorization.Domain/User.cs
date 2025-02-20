using Microsoft.AspNetCore.Identity;

namespace Authorization.Domain
{
    public class User : IdentityUser
    {
        // Additional properties for user profile
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
    }
}
