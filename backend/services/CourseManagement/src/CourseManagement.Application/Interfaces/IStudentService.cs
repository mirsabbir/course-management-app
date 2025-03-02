using CourseManagement.Application.DTOs;
using CourseManagement.Application.DTOs.Classes;
using CourseManagement.Application.DTOs.Courses;
using CourseManagement.Application.DTOs.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Application.Interfaces
{
    public interface IStudentService
    {
        Task<(IEnumerable<StudentDTO> Students, int TotalCount)> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<StudentDTO> GetByIdAsync(Guid id);
        Task<StudentDTO> GetStudentInfoAsync();
        Task<StudentDTO> CreateAsync(CreateStudentDTO createStudentDTO);
        Task<StudentDTO> UpdateAsync(UpdateStudentDTO updateStudentDTO);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<StudentDTO>> SearchStudentsAsync(string searchTerm);
        Task<IEnumerable<CourseDTO>> GetCoursesAsync(Guid studentId);
        Task<IEnumerable<AssignedClassForStudentDTO>> GetAllAssignedClassesAsync(Guid studentId);
        Task<IEnumerable<StudentDTO>> GetClassmates(Guid studentId, Guid classId);
    }
}
