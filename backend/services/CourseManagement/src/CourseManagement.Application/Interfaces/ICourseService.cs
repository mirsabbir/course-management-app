using CourseManagement.Application.DTOs;
using CourseManagement.Application.DTOs.Classes;
using CourseManagement.Application.DTOs.Courses;
using CourseManagement.Application.DTOs.Enrollment;
using CourseManagement.Application.DTOs.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Application.Interfaces
{
    public interface ICourseService
    {
        Task<(IEnumerable<CourseDTO> Courses, int TotalCount)> GetAllCoursesAsync(int pageNumber = 1, int pageSize = 10);
        Task<CourseDTO> GetCourseByIdAsync(Guid id);
        Task<CourseDTO> CreateCourseAsync(CreateCourseDTO createCourseDTO);
        Task<CourseDTO> UpdateCourseAsync(UpdateCourseDTO updateCourseDTO);
        Task DeleteCourseAsync(Guid id);
        Task<IEnumerable<ClassDTO>> GetClassesAsync(Guid id);
        Task<IEnumerable<StudentDTO>> GetStudentsAsync(Guid id);
        Task EnrollStudentAsync(CourseEnrollmentDTO courseEnrollmentDTO);
        Task UnenrollStudentAsync(CourseEnrollmentDTO courseEnrollmentDTO);
    }
}
