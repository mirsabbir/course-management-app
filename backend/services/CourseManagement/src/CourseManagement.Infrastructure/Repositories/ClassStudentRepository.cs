using CourseManagement.Application.DTOs.Students;
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

        public async Task<IEnumerable<ClassStudent>> GetStudentsByClassIdAsync(Guid classId)
        {
            return await _context.ClassStudents
                .Where(cs => cs.ClassId == classId)
                .Include(cs => cs.Student) // Load student details
                .ToListAsync();
        }

        public async Task<IEnumerable<ClassStudent>> GetClasssesByStudentIdAsync(Guid studentId)
        {
            return await _context.ClassStudents
                .Include(cs => cs.Class) // Load class details
                .Where(cs => cs.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetStudentsDirectlyOrIndirectlyAssignedToClassAsync(Guid classId, Guid studentId)
        {
            // Get students directly assigned to the class
            var directClassStudents = from cs in _context.ClassStudents
                                      join s in _context.Students on cs.StudentId equals s.Id
                                      where cs.ClassId == classId
                                      select s;

            // Get students assigned via courses
            var courseStudents = from cs in _context.CourseStudents
                                 join s in _context.Students on cs.StudentId equals s.Id
                                 join cc in _context.ClassCourses on cs.CourseId equals cc.CourseId
                                 where cc.ClassId == classId
                                 select s;

            // Union the results and exclude the requesting student
            var allStudents = directClassStudents
                .Union(courseStudents) // Merge both direct and course-based students
                .Where(s => s.Id != studentId); // Exclude the requesting student

            return await allStudents.ToListAsync();
        }
    }
}
