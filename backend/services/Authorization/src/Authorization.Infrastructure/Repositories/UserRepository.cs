using Authorization.Application.Interfaces;
using Authorization.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authorization.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Method to get all users
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        // Method to get a user by their ID
        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users
                                 .FirstOrDefaultAsync(u => u.Id == userId.ToString());
        }
    }
}
