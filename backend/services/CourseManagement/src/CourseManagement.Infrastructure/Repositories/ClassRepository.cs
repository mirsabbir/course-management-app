using CourseManagement.Application.Interfaces;
using CourseManagement.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Infrastructure.Repositories
{
    public class ClassRepository : IClassRepository
    {
        private readonly ApplicationDbContext _context;

        public ClassRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Class>> GetAllAsync()
        {
            return await _context.Classes.ToListAsync();
        }

        // Get paged classes based on pageNumber and pageSize
        public async Task<IEnumerable<Class>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.Classes
                .OrderByDescending(c => c.CreatedAt) // Ordering classes, could be by name or other property
                .Skip((pageNumber - 1) * pageSize) // Skips items based on page number
                .Take(pageSize) // Limits the number of items to the page size
                .ToListAsync(); // Executes the query and returns the results
        }

        // Get the total count of classes
        public async Task<int> CountAsync()
        {
            return await _context.Classes.CountAsync(); // Returns the total count of classes in the database
        }

        public async Task<Class?> GetByIdAsync(Guid id)
        {
            return await _context.Classes.FindAsync(id);
        }

        public async Task AddAsync(Class classEntity)
        {
            await _context.Classes.AddAsync(classEntity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Class classEntity)
        {
            _context.Classes.Update(classEntity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var classEntity = await _context.Classes.FindAsync(id);
            if (classEntity != null)
            {
                _context.Classes.Remove(classEntity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ClassCourse>> GetByCourseIdAsync(Guid courseId)
        {
            return await _context.ClassCourses
                .Include(cc => cc.Class)
                .Where(cc => cc.CourseId == courseId)
                .ToListAsync();
        }

        // Check if a class with the given name exists
        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Classes.AnyAsync(c => c.Name == name);
        }

        public async Task<IEnumerable<Class>> SearchAsync(string query)
        {
            return await _context.Classes
                .Where(c => c.Name.ToLower().Contains(query.ToLower())
                         || c.Description.ToLower().Contains(query.ToLower()))
                .ToListAsync();
        }

    }
}
