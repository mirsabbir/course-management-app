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
    public class CourseStudentRepository : ICourseStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseStudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(Guid courseId, Guid studentId)
        {
            return await _context.CourseStudents
                .AnyAsync(cs => cs.CourseId == courseId && cs.StudentId == studentId);
        }

        public async Task AddAsync(CourseStudent courseStudent)
        {
            await _context.CourseStudents.AddAsync(courseStudent);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Guid courseId, Guid studentId)
        {
            var enrollment = await _context.CourseStudents
                .FirstOrDefaultAsync(cs => cs.CourseId == courseId && cs.StudentId == studentId);

            if (enrollment != null)
            {
                _context.CourseStudents.Remove(enrollment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Student>> GetStudentsByCourseIdAsync(Guid courseId)
        {
            return await _context.CourseStudents
                    .Where(cs => cs.CourseId == courseId)
                    .Include(cs => cs.Student) // Load student details
                    .Select(cs => cs.Student)
                    .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetCoursesByStudentIdAsync(Guid studentId)
        {
            return await _context.CourseStudents
                .Where(cs => cs.StudentId == studentId)
                .Include(cs => cs.Course) // Load course details
                .Select(cs => cs.Course)
                .ToListAsync();
        }
    }
}
