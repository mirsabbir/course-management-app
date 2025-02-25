using CourseManagement.Application.DTOs.Students;
using CourseManagement.Application.Exceptions;
using CourseManagement.Application.Interfaces;
using CourseManagement.Domain;
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

        public StudentService(
            IStudentRepository studentRepository,
            IUserService userService,
            ILogger<StudentService> logger)
        {
            _studentRepository = studentRepository;
            _userService = userService;
            _logger = logger;
        }

        public async Task<StudentDTO> CreateAsync(CreateStudentDTO createStudentDTO)
        {
            // Log the start of the student creation process with relevant details
            _logger.LogInformation("Creating a new student. FullName: {FullName}, Email: {Email}, DateOfBirth: {DateOfBirth}",
                createStudentDTO.FullName, createStudentDTO.Email, createStudentDTO.DateOfBirth);

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
                DateOfBirth = createStudentDTO.DateOfBirth,
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
                var student = await _studentRepository.GetStudentByIdAsync(id);

                if (student == null)
                {
                    _logger.LogWarning("Student with ID {StudentId} not found", id);
                    throw new NotFoundException("Student not found.");
                }

                await _studentRepository.DeleteStudentAsync(id);
                _logger.LogInformation("Successfully deleted student with ID {StudentId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete student with ID {StudentId}", id);
                throw; // Re-throw the exception after logging
            }
        }

        public async Task<IEnumerable<StudentDTO>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all students");

            try
            {
                var students = await _studentRepository.GetAllStudentsAsync();

                var result = students.Select(s => new StudentDTO
                {
                    Id = s.Id,
                    Email = s.Email,
                    FullName = s.FullName,
                    DateOfBirth = s.DateOfBirth,
                }).ToList();

                _logger.LogInformation("Successfully fetched {StudentCount} students", result.Count);
                return result;
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
            student.DateOfBirth = updateStudentDTO.DateOfBirth;

            await _studentRepository.UpdateStudentAsync(student);

            return new StudentDTO
            {
                Id = student.Id,
                DateOfBirth = student.DateOfBirth,
                FullName = student.FullName,
                Email = student.Email
            };
        }
    }
}
