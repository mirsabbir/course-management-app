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
    public class ClassCourseRepository : IClassCourseRepository
    {
        private readonly ApplicationDbContext _context;

        public ClassCourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(Guid courseId, Guid classId)
        {
            return await _context.ClassCourses
                .AnyAsync(cc => cc.CourseId == courseId && cc.ClassId == classId);
        }

        public async Task AddAsync(ClassCourse classCourse)
        {
            await _context.ClassCourses.AddAsync(classCourse);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Guid courseId, Guid classId)
        {
            var classCourse = await _context.ClassCourses
                .FirstOrDefaultAsync(cc => cc.CourseId == courseId && cc.ClassId == classId);

            if (classCourse != null)
            {
                _context.ClassCourses.Remove(classCourse);
                await _context.SaveChangesAsync();
            }
        }
    }

}
