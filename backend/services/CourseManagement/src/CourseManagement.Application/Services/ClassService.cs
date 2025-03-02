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
        IClassStudentRepository classStudentRepository,
        ILogger<ClassService> logger) : IClassService
    {
        private readonly IClassRepository _classRepository = classRepository;
        private readonly ICourseRepository _courseRepository = courseRepository;
        private readonly IStudentRepository _studentRepository = studentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IUserService _userService = userService;
        private readonly IClassStudentRepository _classStudentRepository = classStudentRepository;
        private readonly ILogger<ClassService> _logger = logger;

        public async Task<ClassDTO> CreateClassAsync(CreateClassDTO createClassDTO)
        {
            // Start logging the creation of a new class
            _logger.LogInformation("Creating a new class with Name: {ClassName}, Description: {ClassDescription}",
                createClassDTO.Name, createClassDTO.Description);

            // Check if a class with the same name already exists
            bool classExists = await _classRepository.ExistsByNameAsync(createClassDTO.Name);
            if (classExists)
            {
                _logger.LogWarning("Class creation failed. A class with the name '{ClassName}' already exists.", createClassDTO.Name);
                throw new InvalidOperationException($"A class with the name '{createClassDTO.Name}' already exists.");
            }

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
                // Check if student exists
                var student = await _studentRepository.GetStudentByIdAsync(classEnrollmentDTO.StudentId);
                if (student == null)
                {
                    _logger.LogWarning("Student with ID {StudentId} not found", classEnrollmentDTO.StudentId);
                    throw new NotFoundException($"Student with ID {classEnrollmentDTO.StudentId} was not found.");
                }

                // Check if class exists
                var @class = await _classRepository.GetByIdAsync(classEnrollmentDTO.ClassId);
                if (@class == null)
                {
                    _logger.LogWarning("Class with ID {ClassId} not found", classEnrollmentDTO.ClassId);
                    throw new NotFoundException($"Class with ID {classEnrollmentDTO.ClassId} was not found.");
                }

                // Check if student is already enrolled
                var isAlreadyEnrolled = await _classStudentRepository.ExistsAsync(classEnrollmentDTO.ClassId, classEnrollmentDTO.StudentId);
                if (isAlreadyEnrolled)
                {
                    _logger.LogWarning(
                        "Student with ID {StudentId} is already enrolled in class with ID {ClassId}",
                        classEnrollmentDTO.StudentId,
                        classEnrollmentDTO.ClassId
                    );
                    throw new InvalidOperationException("Student is already enrolled in this class.");
                }

                // Create enrollment entry
                var classStudent = new ClassStudent
                {
                    ClassId = classEnrollmentDTO.ClassId,
                    StudentId = classEnrollmentDTO.StudentId,
                    AssignedById = GetCurrentUserId(),
                    AssignedByName = GetCurrentUserName(),
                    AssignedAt = DateTime.UtcNow,
                };

                await _classStudentRepository.AddAsync(classStudent);

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


        public async Task<(IEnumerable<ClassDTO> Classes, int TotalCount)> GetAllClassesAsync(int pageNumber = 1, int pageSize = 10)
        {
            var totalCount = await _classRepository.CountAsync(); // Get total count of classes
            var classes = await _classRepository.GetPagedAsync(pageNumber, pageSize); // Get paginated classes

            var classDTOs = classes.Select(c => new ClassDTO
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedByName, // Assuming you have a CreatedByName property in Class entity
            }).ToList();

            return (classDTOs, totalCount); // Return both paged data and the total count
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

        public async Task<IEnumerable<AssignedCourseDTO>> GetCoursesAsync(Guid classId)
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

                var classCourses = await _courseRepository.GetByClassIdAsync(classId);

                var result = classCourses.Select(cc => new AssignedCourseDTO
                {
                    Id = cc.Course.Id,
                    Name = cc.Course.Name,
                    CreatedBy = cc.Course.CreatedByName,
                    CreatedAt = cc.Course.CreatedAt,
                    Description = cc.Course.Description,
                    AssignedAt = cc.AssignedAt,
                    AssignedBy = cc.AssignedByName
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

        public async Task<IEnumerable<AssignedStudentDTO>> GetStudentsAsync(Guid classId)
        {
            _logger.LogInformation("Fetching students for class with ID {ClassId}", classId);

            try
            {
                var classEntity = await _classRepository.GetByIdAsync(classId);

                if (classEntity == null)
                {
                    _logger.LogWarning("Class with ID {ClassId} not found", classId);
                    throw new NotFoundException("Class not found.");
                }

                var enrolledStudents = await _classStudentRepository.GetStudentsByClassIdAsync(classId);

                var result = enrolledStudents.Select(cs => new AssignedStudentDTO
                {
                    Id = cs.Student.Id,
                    Email = cs.Student.Email,
                    DateOfBirth = cs.Student.DateOfBirth,
                    FullName = cs.Student.FullName,
                    CreatedAt = cs.Student.CreatedAt,
                    CreatedBy = cs.Student.CreatedByName,
                    AssignedAt = cs.AssignedAt,
                    AssignedBy = cs.AssignedByName
                }).ToList();

                _logger.LogInformation("Successfully fetched {StudentCount} students for class with ID {ClassId}", result.Count, classId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch students for class with ID {ClassId}", classId);
                throw; // Re-throw the exception after logging
            }
        }

        public async Task UnenrollStudentAsync(ClassEnrollmentDTO classEnrollmentDTO)
        {
            _logger.LogInformation("Unenrolling student {StudentId} from class {ClassId}",
                classEnrollmentDTO.StudentId, classEnrollmentDTO.ClassId);

            try
            {
                var student = await _studentRepository.GetStudentByIdAsync(classEnrollmentDTO.StudentId);
                var classEntity = await _classRepository.GetByIdAsync(classEnrollmentDTO.ClassId);

                if (student == null || classEntity == null)
                {
                    _logger.LogWarning("Invalid student ({StudentId}) or class ({ClassId})",
                        classEnrollmentDTO.StudentId, classEnrollmentDTO.ClassId);
                    throw new NotFoundException("Invalid student or class.");
                }

                // Check if the student is enrolled in the class
                bool isEnrolled = await _classStudentRepository.ExistsAsync(classEntity.Id, student.Id);
                if (!isEnrolled)
                {
                    _logger.LogWarning("Student {StudentId} is not enrolled in class {ClassId}",
                        classEnrollmentDTO.StudentId, classEnrollmentDTO.ClassId);
                    throw new InvalidOperationException("Student is not enrolled in this class.");
                }

                // Unenroll the student by removing the ClassStudent entry
                await _classStudentRepository.RemoveAsync(classEntity.Id, student.Id);

                _logger.LogInformation("Successfully unenrolled student {StudentId} from class {ClassId}",
                    classEnrollmentDTO.StudentId, classEnrollmentDTO.ClassId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to unenroll student with ID {StudentId} from class with ID {ClassId}",
                    classEnrollmentDTO.StudentId,
                    classEnrollmentDTO.ClassId
                );
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

        public async Task<IEnumerable<Class>> SearchClassesAsync(string query)
        {
            return await _classRepository.SearchAsync(query);
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
