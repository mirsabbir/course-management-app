﻿using CourseManagement.Application.DTOs.Classes;
using CourseManagement.Application.DTOs.Courses;
using CourseManagement.Application.DTOs.Enrollment;
using CourseManagement.Application.DTOs.Students;

namespace CourseManagement.Application.Interfaces
{
    public interface IClassService
    {
        Task<IEnumerable<ClassDTO>> GetAllClassesAsync();
        Task<ClassDTO> GetClassByIdAsync(Guid id);
        Task<ClassDTO> CreateClassAsync(CreateClassDTO createClassDTO);
        Task<ClassDTO> UpdateClassAsync(UpdateClassDTO updateClassDTO);
        Task DeleteClassAsync(Guid id);
        Task<IEnumerable<CourseDTO>> GetCoursesAsync(Guid id);
        Task<IEnumerable<StudentDTO>> GetStudentsAsync(Guid id);
        Task EnrollStudentAsync(ClassEnrollmentDTO classEnrollmentDTO);
    }
}
