using CourseManagement.Application.DTOs.Classes;
using CourseManagement.Application.DTOs.Courses;
using CourseManagement.Application.DTOs.Enrollment;
using CourseManagement.Application.DTOs.Students;
using CourseManagement.Application.Exceptions;
using CourseManagement.Application.Interfaces;
using CourseManagement.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CourseManagement.Application.Services
{
    public class ClassService(
        IClassRepository classRepository,
        ICourseRepository courseRepository,
        IStudentRepository studentRepository,
        IHttpContextAccessor httpContextAccessor,
        IUserService userService,
        ILogger<ClassService> logger) : IClassService
    {
        private readonly IClassRepository _classRepository = classRepository;
        private readonly ICourseRepository _courseRepository = courseRepository;
        private readonly IStudentRepository _studentRepository = studentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IUserService _userService = userService;
        private readonly ILogger<ClassService> _logger = logger;

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
                CreatedById = GetCurrentUserId(),
                CreatedByName = GetCurrentUserName(),
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

            var classEntity = await _classRepository.GetByIdAsync(id);
            if (classEntity == null)
            {
                _logger.LogWarning("Class with ID {ClassId} not found", id);
                throw new NotFoundException($"Class with ID {id} was not found.");
            }

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
                if (student == null)
                {
                    _logger.LogWarning("Student with ID {StudentId} not found", classEnrollmentDTO.StudentId);
                    throw new NotFoundException($"Student with ID {classEnrollmentDTO.StudentId} was not found.");
                }

                var classEntity = await _classRepository.GetByIdAsync(classEnrollmentDTO.ClassId);
                if (classEntity == null)
                {
                    _logger.LogWarning("Class with ID {ClassId} not found", classEnrollmentDTO.ClassId);
                    throw new NotFoundException($"Class with ID {classEnrollmentDTO.ClassId} was not found.");
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
                    AssignedById = GetCurrentUserId(),
                    AssignedByName = GetCurrentUserName()
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

                var result = classes.Select(c => new ClassDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    CreatedAt = c.CreatedAt,
                    Description = c.Description,
                    CreatedBy = c.CreatedByName,
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
                    throw new NotFoundException($"Class with ID {id} was not found.");
                }

                _logger.LogInformation("Successfully fetched class with ID {ClassId}", id);

                return new ClassDTO
                {
                    Id = classEntity.Id,
                    Name = classEntity.Name,
                    CreatedAt = classEntity.CreatedAt,
                    CreatedBy = classEntity.CreatedByName,
                    Description = classEntity.Description
                };
            }
            catch (NotFoundException) // Let NotFoundExceptions propagate as-is
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch class with ID {ClassId}", id);
                throw; // Preserve stack trace
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
                    throw new NotFoundException("Class not found.");
                }

                var courses = await _courseRepository.GetByClassIdAsync(classId);

                var result = courses.Select(c => new CourseDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    CreatedBy = c.CreatedByName,
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
                    throw new NotFoundException("Class not found.");
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
                    throw new NotFoundException("Class not found.");
                }

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
                    CreatedBy = classEntity.CreatedByName,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update class with ID {ClassId}", updateClassDTO.Id);
                throw; // Re-throw the exception after logging
            }
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        private string GetCurrentUserName()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst("fullName")?.Value ?? string.Empty;
        }
    }
}
