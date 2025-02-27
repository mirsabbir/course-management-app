using CourseManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Application.Interfaces
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<IEnumerable<Student>> GetPagedAsync(int pageNumber, int pageSize);
        Task<int> CountAsync();
        Task<Student> GetStudentByIdAsync(Guid studentId);
        Task AddStudentAsync(Student student);
        Task UpdateStudentAsync(Student student);
        Task DeleteStudentAsync(Guid studentId);
    }
}
