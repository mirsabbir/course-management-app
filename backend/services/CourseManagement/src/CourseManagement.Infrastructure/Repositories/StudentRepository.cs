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
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _context.Students.ToListAsync();
        }

        // Get paged students based on pageNumber and pageSize
        public async Task<IEnumerable<Student>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.Students
                .OrderByDescending(s => s.CreatedAt) // You can adjust the ordering based on requirements
                .Skip((pageNumber - 1) * pageSize) // Skip based on page number
                .Take(pageSize) // Take only the number of students specified by pageSize
                .ToListAsync(); // Execute the query and return the result
        }

        // Get the total count of students for pagination metadata
        public async Task<int> CountAsync()
        {
            return await _context.Students.CountAsync(); // Count the total number of students
        }

        public async Task<Student?> GetStudentByIdAsync(Guid studentId)
        {
            return await _context.Students
                .FirstOrDefaultAsync(s => s.Id == studentId);
        }

        public async Task AddStudentAsync(Student student)
        {
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStudentAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStudentAsync(Guid studentId)
        {
            var student = await GetStudentByIdAsync(studentId);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
        }

        // Check if a student with the given email exists (case-insensitive check)
        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Students.AnyAsync(s => s.Email.ToLower() == email.ToLower());
        }

        public async Task<IEnumerable<Student>> SearchStudentsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<Student>();
            }

            searchTerm = searchTerm.ToLower(); // Normalize input

            return await _context.Students
                .Where(s => s.FullName.ToLower().Contains(searchTerm) || s.Email.ToLower().Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<Student> GetStudentByUserIdAsync(Guid userId)
        {
            return await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }
    }
}
