using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authorization.Infrastructure
{
    public class RoleSeed
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
    }

    public class UserSeed
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Password { get; set; }
    }

    public class UserRoleSeed
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }

    public class SeedData
    {
        public List<RoleSeed> Roles { get; set; }
        public List<UserSeed> Users { get; set; }
        public List<UserRoleSeed> UserRoles { get; set; }
    }

}
