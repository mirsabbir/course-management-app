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
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IClassRepository _classRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CourseService(
            ICourseRepository courseRepository, 
            IClassRepository classRepository, 
            IStudentRepository studentRepository,
            IUserService userService,
            IHttpContextAccessor httpContextAccessor)
        {
            _courseRepository = courseRepository;
            _classRepository = classRepository;
            _studentRepository = studentRepository;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CourseDTO> CreateCourseAsync(CreateCourseDTO createCourseDTO)
        {
            var newCourse = new Course
            {
                Id = Guid.NewGuid(),
                Name = createCourseDTO.Name,
                Description = createCourseDTO.Description,
                CreatedAt = DateTime.UtcNow,
                CreatedById = GetCurrentUserId() ?? Guid.Empty,
            };

            await _courseRepository.AddAsync(newCourse);

            return new CourseDTO
            {
                Id = newCourse.Id,
                Name = newCourse.Name,
                Description = newCourse.Description,
                CreatedAt = newCourse.CreatedAt,
                CreatedBy = GetCurrentUserName(),
            };
        }

        public async Task DeleteCourseAsync(Guid id)
        {
            await _courseRepository.DeleteAsync(id);
        }

        public async Task EnrollStudentAsync(CourseEnrollmentDTO courseEnrollmentDTO)
        {
            var student = await _studentRepository.GetStudentByIdAsync(courseEnrollmentDTO.StudentId);
            var course = await _courseRepository.GetByIdAsync(courseEnrollmentDTO.CourseId);

            if (student == null || course == null)
                throw new ArgumentException("Invalid student or course.");

            // Check if the student is already enrolled in the course
            if (course.CourseStudents.Any(cs => cs.StudentId == courseEnrollmentDTO.StudentId))
                throw new InvalidOperationException("Student is already enrolled in this course.");

            // Create a new CourseStudent entry
            var courseStudent = new CourseStudent
            {
                CourseId = courseEnrollmentDTO.CourseId,
                StudentId = courseEnrollmentDTO.StudentId,
                AssignedBy = GetCurrentUserId() ?? Guid.Empty
            };

            // Add to the course entity's navigation property
            course.CourseStudents.Add(courseStudent);

            // Persist changes to the database
            await _courseRepository.UpdateAsync(course);
        }


        public async Task<IEnumerable<CourseDTO>> GetAllCoursesAsync()
        {
            var courses = await _courseRepository.GetAllAsync();
            var users = await _userService.GetAllUsersAsync();

            return courses.Select(c => new CourseDTO
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CreatedAt = c.CreatedAt,
                CreatedBy = users.FirstOrDefault(u => u.Id == c.CreatedById)?.FullName ?? "Unknown"
            });
        }

        public async Task<CourseDTO> GetCourseByIdAsync(Guid id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            var user = await _userService.GetUserByIdAsync(course.CreatedById);

            if (course == null)
                throw new KeyNotFoundException("Course not found.");

            return new CourseDTO
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                CreatedAt = course.CreatedAt,
                CreatedBy = user.FullName,
            };
        }

        public async Task<IEnumerable<ClassDTO>> GetClassesAsync(Guid courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            var users = await _userService.GetAllUsersAsync();

            if (course == null)
                throw new KeyNotFoundException("Course not found.");

            var classes = await _classRepository.GetByCourseIdAsync(courseId);

            return classes.Select(c => new ClassDTO
                          {
                              Id = c.Id,
                              Name = c.Name,
                              CreatedBy = users.FirstOrDefault(u => u.Id == c.CreatedById)?.FullName ?? "Unknown",
                              CreatedAt = c.CreatedAt,
                              Description = c.Description
                          });
        }

        public async Task<IEnumerable<StudentDTO>> GetStudentsAsync(Guid id)
        {
            var course = await _courseRepository.GetByIdAsync(id);

            if (course == null)
                throw new KeyNotFoundException("Course not found.");

            // Assuming we already have a list of CourseStudent entities that associate students to courses
            var courseStudents = course.CourseStudents;

            // Extract student details from the course-student relationships
            var enrolledStudents = courseStudents.Select(cs => cs.Student);

            return enrolledStudents.Select(s => new StudentDTO
            {
                Id = s.Id,
                FullName = s.FullName,
                DateOfBirth = s.DateOfBirth,
                Email = s.Email
            });
        }

        public async Task<CourseDTO> UpdateCourseAsync(UpdateCourseDTO updateCourseDTO)
        {
            var course = await _courseRepository.GetByIdAsync(updateCourseDTO.Id);
            var user = await _userService.GetUserByIdAsync(course.CreatedById);
            if (course == null)
                throw new KeyNotFoundException("Course not found.");

            course.Name = updateCourseDTO.Name;
            course.Description = updateCourseDTO.Description;

            await _courseRepository.UpdateAsync(course);

            return new CourseDTO
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                CreatedAt = course.CreatedAt,
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
