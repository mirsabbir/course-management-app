using CourseManagement.Application.DTOs.Classes;
using CourseManagement.Application.DTOs.Courses;
using CourseManagement.Application.DTOs.Enrollment;
using CourseManagement.Application.DTOs.Students;
using CourseManagement.Domain;

namespace CourseManagement.Application.Interfaces
{
    public interface IClassService
    {
        Task<(IEnumerable<ClassDTO> Classes, int TotalCount)> GetAllClassesAsync(int pageNumber = 1, int pageSize = 10);
        Task<ClassDTO> GetClassByIdAsync(Guid id);
        Task<ClassDTO> CreateClassAsync(CreateClassDTO createClassDTO);
        Task<ClassDTO> UpdateClassAsync(UpdateClassDTO updateClassDTO);
        Task DeleteClassAsync(Guid id);
        Task<IEnumerable<AssignedCourseDTO>> GetCoursesAsync(Guid id);
        Task<IEnumerable<AssignedStudentDTO>> GetStudentsAsync(Guid id);
        Task EnrollStudentAsync(ClassEnrollmentDTO classEnrollmentDTO);
        Task UnenrollStudentAsync(ClassEnrollmentDTO classEnrollmentDTO);
        Task<IEnumerable<Class>> SearchClassesAsync(string query);
    }
}
