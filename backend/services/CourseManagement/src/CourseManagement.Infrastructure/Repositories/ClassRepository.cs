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

        public async Task<IEnumerable<Class>> GetByCourseIdAsync(Guid courseId)
        {
            return await _context.Classes
                .Where(c => c.ClassCourses.Any(cc => cc.CourseId == courseId))
                .ToListAsync();
        }
    }
}
