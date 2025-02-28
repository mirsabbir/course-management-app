using Authorization.Domain;
using Authorization.Infrastructure.EntityTypeConfigurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Authorization.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Invitation> Invitations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
            //Load seed data from JSON file
            var seedData = LoadSeedData();

            // Seed roles
            builder.Entity<IdentityRole>().HasData(seedData.Roles.Select(r => new IdentityRole
            {
                Id = r.Id,
                Name = r.Name,
                NormalizedName = r.NormalizedName
            }));
            
            // Seed users
            builder.Entity<User>().HasData(seedData.Users.Select(u => new User
            {
                Id = u.Id,
                FullName = u.FullName,
                DateOfBirth = u.DateOfBirth.ToUniversalTime(),
                UserName = u.UserName,
                NormalizedUserName = u.NormalizedUserName,
                Email = u.Email,
                NormalizedEmail = u.NormalizedEmail,
                EmailConfirmed = u.EmailConfirmed,
                PasswordHash = u.Password,
                SecurityStamp = "bhdbcvhdbchdbhjkbs15c1sdf51",
                ConcurrencyStamp = "adcsdcsdc25d1fv5fd"
            }));


            builder.Entity<IdentityUserRole<string>>().HasData(seedData.UserRoles.Select(ur => new IdentityUserRole<string>
            {
                RoleId = ur.RoleId,
                UserId = ur.UserId
            }));


        }

        private SeedData LoadSeedData()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "seedData.json");
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<SeedData>(json);
        }
    }
}
