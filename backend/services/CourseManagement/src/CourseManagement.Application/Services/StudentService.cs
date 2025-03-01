using CourseManagement.Application.DTOs.Classes;
using CourseManagement.Application.DTOs.Courses;
using CourseManagement.Application.DTOs.Students;
using CourseManagement.Application.Exceptions;
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
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUserService _userService;
        private readonly ILogger<StudentService> _logger;
        private readonly IClassStudentRepository _classStudentRepository;
        private readonly ICourseStudentRepository _courseStudentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StudentService(
            IStudentRepository studentRepository,
            IUserService userService,
            ILogger<StudentService> logger,
            IClassStudentRepository classStudentRepository,
            ICourseStudentRepository courseStudentRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _studentRepository = studentRepository;
            _userService = userService;
            _logger = logger;
            _classStudentRepository = classStudentRepository;
            _courseStudentRepository = courseStudentRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<StudentDTO> CreateAsync(CreateStudentDTO createStudentDTO)
        {
            // Log the start of the student creation process with relevant details
            _logger.LogInformation("Creating a new student. FullName: {FullName}, Email: {Email}, DateOfBirth: {DateOfBirth}",
                createStudentDTO.FullName, createStudentDTO.Email, createStudentDTO.DateOfBirth);

            // Check if a student with the same email already exists
            bool studentExists = await _studentRepository.ExistsByEmailAsync(createStudentDTO.Email);
            if (studentExists)
            {
                _logger.LogWarning("Student creation failed. A student with the email '{StudentEmail}' already exists.", createStudentDTO.Email);
                throw new InvalidOperationException($"A student with the email '{createStudentDTO.Email}' already exists.");
            }

            // Attempt to create a new user for the student
            string userId = null;
            try
            {
                userId = await _userService.CreateUserAsync(new DTOs.CreateUserDTO
                {
                    Email = createStudentDTO.Email,
                    FullName = createStudentDTO.FullName,
                });

                // Log successful user creation
                _logger.LogInformation("User created successfully. UserId: {UserId}, FullName: {FullName}, Email: {Email}",
                    userId, createStudentDTO.FullName, createStudentDTO.Email);
            }
            catch (Exception ex)
            {
                // Log any error that occurs during user creation
                _logger.LogError(ex, "Error occurred while creating user for student. FullName: {FullName}, Email: {Email}, Error: {ErrorMessage}",
                    createStudentDTO.FullName, createStudentDTO.Email, ex.Message);
                throw; // Rethrow the exception after logging it
            }

            // Create the new student entity
            var newStudent = new Student
            {
                Id = Guid.NewGuid(),
                FullName = createStudentDTO.FullName,
                Email = createStudentDTO.Email,
                DateOfBirth = createStudentDTO.DateOfBirth.ToUniversalTime(),
                UserId = Guid.Parse(userId), // Convert string userId to Guid
                CreatedByName = string.Empty
            };

            // Log the new student creation
            _logger.LogInformation("New student created. StudentId: {StudentId}, FullName: {FullName}, Email: {Email}, DateOfBirth: {DateOfBirth}, UserId: {UserId}",
                newStudent.Id, newStudent.FullName, newStudent.Email, newStudent.DateOfBirth, newStudent.UserId);

            // Attempt to add the student to the repository
            try
            {
                await _studentRepository.AddStudentAsync(newStudent);
                // Log successful student addition to the repository
                _logger.LogInformation("Student successfully added to the repository. StudentId: {StudentId}", newStudent.Id);
            }
            catch (Exception ex)
            {
                // Log any error that occurs during student addition
                _logger.LogError(ex, "Error occurred while adding student to repository. StudentId: {StudentId}, Error: {ErrorMessage}",
                    newStudent.Id, ex.Message);
                throw; // Rethrow the exception after logging it
            }

            // Create the StudentDTO to return
            var studentDTO = new StudentDTO
            {
                Id = newStudent.Id,
                Email = newStudent.Email,
                DateOfBirth = newStudent.DateOfBirth,
                FullName = newStudent.FullName,
            };

            // Log the successful creation of the StudentDTO
            _logger.LogInformation("Returning StudentDTO. StudentId: {StudentId}, FullName: {FullName}, Email: {Email}",
                studentDTO.Id, studentDTO.FullName, studentDTO.Email);

            // Return the StudentDTO
            return studentDTO;
        }


        public async Task DeleteAsync(Guid id)
        {
            _logger.LogInformation("Deleting student with ID {StudentId}", id);

            try
            {
                // Retrieve the student by ID
                var student = await _studentRepository.GetStudentByIdAsync(id);

                if (student == null)
                {
                    _logger.LogWarning("Student with ID {StudentId} not found", id);
                    throw new NotFoundException("Student not found.");
                }

                _logger.LogInformation("Deleting user associated with student ID {StudentId}", id);

                // Call the user service to delete the user
                try
                {
                    await _userService.DeleteUserByIdAsync(student.UserId);
                    _logger.LogInformation("Successfully deleted user for student ID {StudentId}", id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to delete user for student ID {StudentId}", id);
                    throw; // Re-throw the exception after logging
                }

                // Delete the student from the repository
                await _studentRepository.DeleteStudentAsync(id);
                _logger.LogInformation("Successfully deleted student with ID {StudentId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete student with ID {StudentId}", id);
                throw; // Re-throw the exception after logging
            }
        }

        public async Task<(IEnumerable<StudentDTO> Students, int TotalCount)> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation("Fetching all students");

            try
            {
                var totalCount = await _studentRepository.CountAsync(); // Get the total number of students
                var students = await _studentRepository.GetPagedAsync(pageNumber, pageSize); // Get paginated students

                var result = students.Select(s => new StudentDTO
                {
                    Id = s.Id,
                    Email = s.Email,
                    FullName = s.FullName,
                    DateOfBirth = s.DateOfBirth,
                }).ToList();

                _logger.LogInformation("Successfully fetched {StudentCount} students", result.Count);
                return (result, totalCount); // Return both the student data and total count
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch all students");
                throw; // Re-throw the exception after logging
            }
        }

        public async Task<StudentDTO> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Fetching student with ID {StudentId}", id);

            try
            {
                var student = await _studentRepository.GetStudentByIdAsync(id);

                if (student == null)
                {
                    _logger.LogWarning("Student with ID {StudentId} not found", id);
                    throw new NotFoundException("Student not found.");
                }

                var studentDTO = new StudentDTO
                {
                    Id = student.Id,
                    FullName = student.FullName,
                    DateOfBirth = student.DateOfBirth,
                    Email = student.Email
                };

                _logger.LogInformation(
                    "Successfully fetched student with ID {StudentId}: FullName: {FullName}, Email: {Email}",
                    studentDTO.Id,
                    studentDTO.FullName,
                    studentDTO.Email
                );

                return studentDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch student with ID {StudentId}", id);
                throw; // Re-throw the exception after logging
            }
        }

        public async Task<StudentDTO> UpdateAsync(UpdateStudentDTO updateStudentDTO)
        {
            var student = await _studentRepository.GetStudentByIdAsync(updateStudentDTO.Id);

            if (student == null)
                throw new NotFoundException("Student not found.");

            student.FullName = updateStudentDTO.FullName;
            student.DateOfBirth = updateStudentDTO.DateOfBirth.ToUniversalTime();

            await _studentRepository.UpdateStudentAsync(student);

            return new StudentDTO
            {
                Id = student.Id,
                DateOfBirth = student.DateOfBirth.ToUniversalTime(),
                FullName = student.FullName,
                Email = student.Email
            };
        }

        public async Task<IEnumerable<StudentDTO>> SearchStudentsAsync(string searchTerm)
        {
            _logger.LogInformation("Searching students with term: {SearchTerm}", searchTerm);

            var students = await _studentRepository.SearchStudentsAsync(searchTerm);

            return students.Select(s => new StudentDTO
            {
                Id = s.Id,
                FullName = s.FullName,
                Email = s.Email,
            }).ToList();
        }

        public async Task<IEnumerable<CourseDTO>> GetCoursesAsync(Guid studentId)
        {
            var student = await _studentRepository.GetStudentByIdAsync(studentId);

            if (student == null)
                throw new NotFoundException("Student not found.");

            if ((_httpContextAccessor.HttpContext?.User.IsInRole("Student") ?? true)
                && student.UserId != GetCurrentUserId())
            {
                throw new UnauthorizedAccessException("You are not allowed to access.");
            }

            var courses = await _courseStudentRepository.GetCoursesByStudentIdAsync(studentId);

            return courses.Select(course => new CourseDTO
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                CreatedAt = course.CreatedAt,
                CreatedBy = course.CreatedByName
            });
        }

        public async Task<IEnumerable<ClassDTO>> GetClassesAsync(Guid studentId)
        {
            var student = await _studentRepository.GetStudentByIdAsync(studentId);

            if (student == null)
                throw new NotFoundException("Student not found.");

            if ((_httpContextAccessor.HttpContext?.User.IsInRole("Student") ?? true)
                && student.UserId != GetCurrentUserId())
            {
                throw new UnauthorizedAccessException("You are not allowed to access.");
            }

            var classes = await _classStudentRepository.GetClasssesByStudentIdAsync(studentId);

            return classes.Select(classEntity => new ClassDTO
            {
                Id = classEntity.Id,
                Name = classEntity.Name,
                Description = classEntity.Description,
                CreatedAt = classEntity.CreatedAt,
                CreatedBy = classEntity.CreatedByName
            });
        }

        public async Task<StudentDTO> GetStudentInfoAsync()
        {
            var userId = GetCurrentUserId();
            var student = await _studentRepository.GetStudentByUserIdAsync(userId);
            if (student == null)
            {
                throw new NotFoundException("Student Not Found");
            }

            return new StudentDTO
            {
                DateOfBirth = student.DateOfBirth,
                Email = student.Email,
                FullName = student.FullName,
                Id = student.Id,
            };
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
