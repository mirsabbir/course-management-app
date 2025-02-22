using CourseManagement.Application.DTOs.Classes;
using CourseManagement.Application.DTOs.Courses;
using CourseManagement.Application.DTOs.Enrollment;
using CourseManagement.Application.DTOs.Students;
using CourseManagement.Application.Interfaces;
using CourseManagement.Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseManagement.Application.Services
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _classRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;

        public ClassService(
            IClassRepository classRepository,
            ICourseRepository courseRepository, 
            IStudentRepository studentRepository,
            IHttpContextAccessor httpContextAccessor,
            IUserService userService)
        {
            _classRepository = classRepository;
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        public async Task<ClassDTO> CreateClassAsync(CreateClassDTO createClassDTO)
        {
            var newClass = new Class
            {
                Id = Guid.NewGuid(),
                Name = createClassDTO.Name,
                Description = createClassDTO.Description,
                CreatedAt = DateTime.UtcNow,
                CreatedById = GetCurrentUserId() ?? Guid.Empty,
            };

            await _classRepository.AddAsync(newClass);

            return new ClassDTO
            {
                Id = newClass.Id,
                Name = newClass.Name,
                CreatedAt = newClass.CreatedAt,
                Description = newClass.Description,
                CreatedBy = GetCurrentUserName(),
            };
        }

        public async Task DeleteClassAsync(Guid id)
        {
            await _classRepository.DeleteAsync(id);
        }

        public async Task EnrollStudentAsync(ClassEnrollmentDTO classEnrollmentDTO)
        {
            var student = await _studentRepository.GetStudentByIdAsync(classEnrollmentDTO.StudentId);
            var classEntity = await _classRepository.GetByIdAsync(classEnrollmentDTO.ClassId);

            if (student == null || classEntity == null)
                throw new ArgumentException("Invalid student or class.");

            // Check if the student is already enrolled in the class
            if (classEntity.ClassStudents.Any(cs => cs.StudentId == classEnrollmentDTO.StudentId))
                throw new InvalidOperationException("Student is already enrolled in this class.");

            // Create a new ClassStudent entry
            var classStudent = new ClassStudent
            {
                ClassId = classEnrollmentDTO.ClassId,
                StudentId = classEnrollmentDTO.StudentId,
                AssignedBy = GetCurrentUserId() ?? Guid.Empty,
            };

            // Add to the class entity's navigation property
            classEntity.ClassStudents.Add(classStudent);

            // Persist changes to the database
            await _classRepository.UpdateAsync(classEntity);
        }

        public async Task<IEnumerable<ClassDTO>> GetAllClassesAsync()
        {
            var classes = await _classRepository.GetAllAsync();
            var users = await _userService.GetAllUsersAsync();

            return classes.Select(c => new ClassDTO
            {
                Id = c.Id,
                Name = c.Name,
                CreatedAt = c.CreatedAt,
                Description = c.Description,
                CreatedBy = users.FirstOrDefault(u => u.Id == c.CreatedById)?.FullName ?? "Unknown",
            });
        }

        public async Task<ClassDTO> GetClassByIdAsync(Guid id)
        {
            var classEntity = await _classRepository.GetByIdAsync(id);
            var user = await _userService.GetUserByIdAsync(classEntity.CreatedById);

            if (classEntity == null)
                throw new KeyNotFoundException("Class not found.");

            return new ClassDTO
            {
                Id = classEntity.Id,
                Name = classEntity.Name,
                CreatedAt = classEntity.CreatedAt,
                CreatedBy = user.FullName,
                Description = classEntity.Description,
            };
        }

        public async Task<IEnumerable<CourseDTO>> GetCoursesAsync(Guid classId)
        {
            var classEntity = await _classRepository.GetByIdAsync(classId);
            var users = await _userService.GetAllUsersAsync();

            if (classEntity == null)
                throw new KeyNotFoundException("Class not found.");

            var courses = await _courseRepository.GetByClassIdAsync(classId);

            return courses.Select(c => new CourseDTO
                          {
                              Id = c.Id,
                              Name = c.Name,
                              CreatedBy = users.FirstOrDefault(u => u.Id == c.CreatedById)?.FullName ?? "Unknown",
                              CreatedAt = c.CreatedAt,
                              Description = c.Description,
                          });
        }

        public async Task<IEnumerable<StudentDTO>> GetStudentsAsync(Guid id)
        {
            var classEntity = await _classRepository.GetByIdAsync(id);

            if (classEntity == null)
                throw new KeyNotFoundException("Class not found.");

            var classStudents = classEntity.ClassStudents;

            var enrolledStudents = classStudents.Select(cs => cs.Student);

            return enrolledStudents.Select(s => new StudentDTO
            {
                Id = s.Id,
                Email = s.Email,
                DateOfBirth = s.DateOfBirth,
                FullName = s.FullName
            });
        }

        public async Task<ClassDTO> UpdateClassAsync(UpdateClassDTO updateClassDTO)
        {
            var classEntity = await _classRepository.GetByIdAsync(updateClassDTO.Id);
            var user = await _userService.GetUserByIdAsync(classEntity.CreatedById);

            if (classEntity == null)
                throw new KeyNotFoundException("Class not found.");

            classEntity.Name = updateClassDTO.Name;
            classEntity.Description = updateClassDTO.Description;


            await _classRepository.UpdateAsync(classEntity);

            return new ClassDTO
            {
                Id = classEntity.Id,
                Name = classEntity.Name,
                CreatedAt = classEntity.CreatedAt,
                Description = classEntity.Description,
                CreatedBy = user.FullName,
            };
        }
        
        private Guid? GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        private string? GetCurrentUserName()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst("UserName")?.Value;
        }
    }
}
