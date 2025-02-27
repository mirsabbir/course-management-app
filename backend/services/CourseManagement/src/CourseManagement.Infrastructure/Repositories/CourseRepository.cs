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
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _context.Courses.ToListAsync();
        }

        // Get paged courses based on pageNumber and pageSize
        public async Task<IEnumerable<Course>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.Courses
                .OrderByDescending(c => c.CreatedAt) // Or any other ordering logic
                .Skip((pageNumber - 1) * pageSize) // Skip the items based on page number
                .Take(pageSize) // Take only the number of items specified in pageSize
                .ToListAsync(); // Execute the query and return the results as a list
        }

        // Get the total count of courses for pagination metadata
        public async Task<int> CountAsync()
        {
            return await _context.Courses.CountAsync(); // Simply count the total number of records
        }

        public async Task<Course?> GetByIdAsync(Guid id)
        {
            return await _context.Courses.FindAsync(id);
        }

        public async Task AddAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Course>> GetByClassIdAsync(Guid classId)
        {
            return await _context.Courses
                .Where(course => course.ClassCourses.Any(cc => cc.ClassId == classId))
                .ToListAsync();
        }

        // Check if a course with the given name exists (case-insensitive check)
        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Courses.AnyAsync(c => c.Name.ToLower() == name.ToLower());
        }
    }

}
