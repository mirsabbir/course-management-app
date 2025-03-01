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
    public class ClassStudentRepository : IClassStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public ClassStudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(Guid classId, Guid studentId)
        {
            return await _context.ClassStudents
                .AnyAsync(cs => cs.ClassId == classId && cs.StudentId == studentId);
        }

        public async Task AddAsync(ClassStudent classStudent)
        {
            await _context.ClassStudents.AddAsync(classStudent);
            await _context.SaveChangesAsync(); // Persist changes
        }

        public async Task RemoveAsync(Guid classId, Guid studentId)
        {
            var enrollment = await _context.ClassStudents
                .FirstOrDefaultAsync(cs => cs.ClassId == classId && cs.StudentId == studentId);

            if (enrollment != null)
            {
                _context.ClassStudents.Remove(enrollment);
                await _context.SaveChangesAsync(); // Persist changes
            }
        }

        public async Task<IEnumerable<Student>> GetStudentsByClassIdAsync(Guid classId)
        {
            return await _context.ClassStudents
                .Where(cs => cs.ClassId == classId)
                .Include(cs => cs.Student) // Load student details
                .Select(cs => cs.Student)
                .ToListAsync();
        }

        public async Task<IEnumerable<Class>> GetClasssesByStudentIdAsync(Guid studentId)
        {
            return await _context.ClassStudents
                .Where(cs => cs.StudentId == studentId)
                .Include(cs => cs.Class) // Load class details
                .Select(cs => cs.Class)
                .ToListAsync();
        }
    }

}
