using CourseManagement.Application.DTOs.Classes;
using CourseManagement.Application.DTOs.Courses;
using CourseManagement.Application.DTOs.Enrollment;
using CourseManagement.Application.DTOs.Students;
using CourseManagement.Application.Interfaces;
using CourseManagement.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ClassService> _logger;

        public ClassService(
            IClassRepository classRepository,
            ICourseRepository courseRepository, 
            IStudentRepository studentRepository,
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            ILogger<ClassService> logger)
        {
            _classRepository = classRepository;
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _logger = logger;
        }

        public async Task<ClassDTO> CreateClassAsync(CreateClassDTO createClassDTO)
        {
            // Start logging the creation of a new class
            _logger.LogInformation("Creating a new class with Name: {ClassName}, Description: {ClassDescription}",
                createClassDTO.Name, createClassDTO.Description);

            var newClass = new Class
            {
                Id = Guid.NewGuid(),
                Name = createClassDTO.Name,
                Description = createClassDTO.Description,
                CreatedAt = DateTime.UtcNow,
                CreatedById = GetCurrentUserId() ?? Guid.Empty,
            };

            // Log the creation of the class entity
            _logger.LogInformation("New Class Created: {ClassId}, CreatedBy: {CreatedBy}, CreatedAt: {CreatedAt}",
                newClass.Id, newClass.CreatedById, newClass.CreatedAt);

            // Perform the repository operation
            try
            {
                await _classRepository.AddAsync(newClass);

                // Log successful creation of class
                _logger.LogInformation("Class with Id {ClassId} successfully added to the repository.", newClass.Id);
            }
            catch (Exception ex)
            {
                // Log error if something goes wrong
                _logger.LogError(ex, "Failed to create class with Name: {ClassName}. Error: {ErrorMessage}",
                    createClassDTO.Name, ex.Message);
                throw; // Optionally rethrow the exception after logging it
            }

            // Return DTO to the caller
            var classDTO = new ClassDTO
            {
                Id = newClass.Id,
                Name = newClass.Name,
                CreatedAt = newClass.CreatedAt,
                Description = newClass.Description,
                CreatedBy = GetCurrentUserName(),
            };

            // Log the returned DTO details
            _logger.LogInformation("Class created successfully with Id: {ClassId}, Name: {ClassName}, CreatedBy: {CreatedBy}",
                classDTO.Id, classDTO.Name, classDTO.CreatedBy);

            return classDTO;
        }


        public async Task DeleteClassAsync(Guid id)
        {
            _logger.LogInformation("Deleting class with ID {ClassId}", id);

            try
            {
                await _classRepository.DeleteAsync(id);
                _logger.LogInformation("Successfully deleted class with ID {ClassId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete class with ID {ClassId}", id);
                throw; // Re-throw the exception after logging
            }
        }

        public async Task EnrollStudentAsync(ClassEnrollmentDTO classEnrollmentDTO)
        {
            _logger.LogInformation(
                "Enrolling student with ID {StudentId} into class with ID {ClassId}",
                classEnrollmentDTO.StudentId,
                classEnrollmentDTO.ClassId
            );

            try
            {
                var student = await _studentRepository.GetStudentByIdAsync(classEnrollmentDTO.StudentId);
                var classEntity = await _classRepository.GetByIdAsync(classEnrollmentDTO.ClassId);

                if (student == null || classEntity == null)
                {
                    _logger.LogWarning(
                        "Invalid student or class. Student ID: {StudentId}, Class ID: {ClassId}",
                        classEnrollmentDTO.StudentId,
                        classEnrollmentDTO.ClassId
                    );
                    throw new ArgumentException("Invalid student or class.");
                }

                if (classEntity.ClassStudents.Any(cs => cs.StudentId == classEnrollmentDTO.StudentId))
                {
                    _logger.LogWarning(
                        "Student with ID {StudentId} is already enrolled in class with ID {ClassId}",
                        classEnrollmentDTO.StudentId,
                        classEnrollmentDTO.ClassId
                    );
                    throw new InvalidOperationException("Student is already enrolled in this class.");
                }

                var classStudent = new ClassStudent
                {
                    ClassId = classEnrollmentDTO.ClassId,
                    StudentId = classEnrollmentDTO.StudentId,
                    AssignedBy = GetCurrentUserId() ?? Guid.Empty,
                };

                classEntity.ClassStudents.Add(classStudent);
                await _classRepository.UpdateAsync(classEntity);

                _logger.LogInformation(
                    "Successfully enrolled student with ID {StudentId} into class with ID {ClassId}",
                    classEnrollmentDTO.StudentId,
                    classEnrollmentDTO.ClassId
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to enroll student with ID {StudentId} into class with ID {ClassId}",
                    classEnrollmentDTO.StudentId,
                    classEnrollmentDTO.ClassId
                );
                throw; // Re-throw the exception after logging
            }
        }

        public async Task<IEnumerable<ClassDTO>> GetAllClassesAsync()
        {
            _logger.LogInformation("Fetching all classes");

            try
            {
                var classes = await _classRepository.GetAllAsync();
                var users = await _userService.GetAllUsersAsync();

                var result = classes.Select(c => new ClassDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    CreatedAt = c.CreatedAt,
                    Description = c.Description,
                    CreatedBy = users.FirstOrDefault(u => u.Id == c.CreatedById)?.FullName ?? "Unknown",
                }).ToList();

                _logger.LogInformation("Successfully fetched {ClassCount} classes", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch all classes");
                throw; // Re-throw the exception after logging
            }
        }

        public async Task<ClassDTO> GetClassByIdAsync(Guid id)
        {
            _logger.LogInformation("Fetching class with ID {ClassId}", id);

            try
            {
                var classEntity = await _classRepository.GetByIdAsync(id);

                if (classEntity == null)
                {
                    _logger.LogWarning("Class with ID {ClassId} not found", id);
                    throw new KeyNotFoundException("Class not found.");
                }

                var user = await _userService.GetUserByIdAsync(classEntity.CreatedById);

                _logger.LogInformation("Successfully fetched class with ID {ClassId}", id);
                return new ClassDTO
                {
                    Id = classEntity.Id,
                    Name = classEntity.Name,
                    CreatedAt = classEntity.CreatedAt,
                    CreatedBy = user.FullName,
                    Description = classEntity.Description,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch class with ID {ClassId}", id);
                throw; // Re-throw the exception after logging
            }
        }

        public async Task<IEnumerable<CourseDTO>> GetCoursesAsync(Guid classId)
        {
            _logger.LogInformation("Fetching courses for class with ID {ClassId}", classId);

            try
            {
                var classEntity = await _classRepository.GetByIdAsync(classId);

                if (classEntity == null)
                {
                    _logger.LogWarning("Class with ID {ClassId} not found", classId);
                    throw new KeyNotFoundException("Class not found.");
                }

                var users = await _userService.GetAllUsersAsync();
                var courses = await _courseRepository.GetByClassIdAsync(classId);

                var result = courses.Select(c => new CourseDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    CreatedBy = users.FirstOrDefault(u => u.Id == c.CreatedById)?.FullName ?? "Unknown",
                    CreatedAt = c.CreatedAt,
                    Description = c.Description,
                }).ToList();

                _logger.LogInformation("Successfully fetched {CourseCount} courses for class with ID {ClassId}", result.Count, classId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch courses for class with ID {ClassId}", classId);
                throw; // Re-throw the exception after logging
            }
        }

        public async Task<IEnumerable<StudentDTO>> GetStudentsAsync(Guid id)
        {
            _logger.LogInformation("Fetching students for class with ID {ClassId}", id);

            try
            {
                var classEntity = await _classRepository.GetByIdAsync(id);

                if (classEntity == null)
                {
                    _logger.LogWarning("Class with ID {ClassId} not found", id);
                    throw new KeyNotFoundException("Class not found.");
                }

                var classStudents = classEntity.ClassStudents;
                var enrolledStudents = classStudents.Select(cs => cs.Student);

                var result = enrolledStudents.Select(s => new StudentDTO
                {
                    Id = s.Id,
                    Email = s.Email,
                    DateOfBirth = s.DateOfBirth,
                    FullName = s.FullName
                }).ToList();

                _logger.LogInformation("Successfully fetched {StudentCount} students for class with ID {ClassId}", result.Count, id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch students for class with ID {ClassId}", id);
                throw; // Re-throw the exception after logging
            }
        }

        public async Task<ClassDTO> UpdateClassAsync(UpdateClassDTO updateClassDTO)
        {
            _logger.LogInformation("Updating class with ID {ClassId}", updateClassDTO.Id);

            try
            {
                var classEntity = await _classRepository.GetByIdAsync(updateClassDTO.Id);

                if (classEntity == null)
                {
                    _logger.LogWarning("Class with ID {ClassId} not found", updateClassDTO.Id);
                    throw new KeyNotFoundException("Class not found.");
                }

                var user = await _userService.GetUserByIdAsync(classEntity.CreatedById);

                // Log the current state of the class before updating
                _logger.LogInformation(
                    "Current class details - Name: {CurrentName}, Description: {CurrentDescription}",
                    classEntity.Name,
                    classEntity.Description
                );

                // Update the class entity
                classEntity.Name = updateClassDTO.Name;
                classEntity.Description = updateClassDTO.Description;

                // Log the updated state of the class
                _logger.LogInformation(
                    "Updated class details - Name: {UpdatedName}, Description: {UpdatedDescription}",
                    updateClassDTO.Name,
                    updateClassDTO.Description
                );

                await _classRepository.UpdateAsync(classEntity);

                _logger.LogInformation("Successfully updated class with ID {ClassId}", updateClassDTO.Id);

                return new ClassDTO
                {
                    Id = classEntity.Id,
                    Name = classEntity.Name,
                    CreatedAt = classEntity.CreatedAt,
                    Description = classEntity.Description,
                    CreatedBy = user.FullName,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update class with ID {ClassId}", updateClassDTO.Id);
                throw; // Re-throw the exception after logging
            }
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        private string? GetCurrentUserName()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst("fullName")?.Value;
        }
    }
}
