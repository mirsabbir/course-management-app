using CourseManagement.Application.DTOs;
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
        Task<IEnumerable<StudentDTO>> GetAllAsync();
        Task<StudentDTO> GetByIdAsync(Guid id);
        Task<StudentDTO> CreateAsync(CreateStudentDTO createStudentDTO);
        Task<StudentDTO> UpdateAsync(UpdateStudentDTO updateStudentDTO);
        Task DeleteAsync(Guid id);
    }
}
