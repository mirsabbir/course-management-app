﻿using CourseManagement.Application.DTOs.Classes;
using CourseManagement.Application.DTOs.Courses;
using CourseManagement.Application.DTOs.Enrollment;
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
    public class CourseService(
        ICourseRepository courseRepository,
        IClassRepository classRepository,
        IStudentRepository studentRepository,
        IUserService userService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<CourseService> logger) : ICourseService
    {
        private readonly ICourseRepository _courseRepository = courseRepository;
        private readonly IClassRepository _classRepository = classRepository;
        private readonly IStudentRepository _studentRepository = studentRepository;
        private readonly IUserService _userService = userService;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ILogger<CourseService> _logger = logger;

        public async Task<CourseDTO> CreateCourseAsync(CreateCourseDTO createCourseDTO)
        {
            // Log the start of the course creation process
            _logger.LogInformation("Starting to create a new course. Name: {CourseName}, Description: {CourseDescription}",
                createCourseDTO.Name, createCourseDTO.Description);

            // Create the new course entity
            var newCourse = new Course
            {
                Id = Guid.NewGuid(),
                Name = createCourseDTO.Name,
                Description = createCourseDTO.Description,
                CreatedAt = DateTime.UtcNow,
                CreatedById = GetCurrentUserId(),
                CreatedByName = GetCurrentUserName()
            };

            // Log the details of the newly created course
            _logger.LogInformation("New course created: {CourseId}, CreatedById: {CreatedById}, CreatedAt: {CreatedAt}",
                newCourse.Id, newCourse.CreatedById, newCourse.CreatedAt);

            // Attempt to add the course to the repository
            try
            {
                await _courseRepository.AddAsync(newCourse);
                // Log success when the course is successfully added to the repository
                _logger.LogInformation("Course with Id {CourseId} successfully added to the repository.", newCourse.Id);
            }
            catch (Exception ex)
            {
                // Log any errors that occur during the course creation process
                _logger.LogError(ex, "Error occurred while adding the course with Name: {CourseName}. Error: {ErrorMessage}",
                    createCourseDTO.Name, ex.Message);
                throw; // Rethrow the exception after logging it
            }

            // Return the created course DTO
            var courseDTO = new CourseDTO
            {
                Id = newCourse.Id,
                Name = newCourse.Name,
                Description = newCourse.Description,
                CreatedAt = newCourse.CreatedAt,
                CreatedBy = GetCurrentUserName(),
            };

            // Log the returned CourseDTO
            _logger.LogInformation("Course creation successful. Returning CourseDTO: {CourseId}, {CourseName}, CreatedBy: {CreatedBy}",
                courseDTO.Id, courseDTO.Name, courseDTO.CreatedBy);

            return courseDTO;
        }

        public async Task DeleteCourseAsync(Guid id)
        {
            _logger.LogInformation("Deleting course with ID {CourseId}", id);

            try
            {
                var course = await _courseRepository.GetByIdAsync(id);
                if (course == null)
                {
                    _logger.LogWarning("Course with ID {CourseId} not found", id);
                    throw new NotFoundException($"Course with ID {id} was not found.");
                }

                await _courseRepository.DeleteAsync(id);

                _logger.LogInformation("Successfully deleted course with ID {CourseId}", id);
            }
            catch (NotFoundException)
            {
                throw; // Allow NotFoundException to propagate as-is
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete course with ID {CourseId}", id);
                throw; // Preserve stack trace
            }
        }

        public async Task EnrollStudentAsync(CourseEnrollmentDTO courseEnrollmentDTO)
        {
            _logger.LogInformation(
                "Enrolling student with ID {StudentId} into course with ID {CourseId}",
                courseEnrollmentDTO.StudentId,
                courseEnrollmentDTO.CourseId
            );

            try
            {
                var student = await _studentRepository.GetStudentByIdAsync(courseEnrollmentDTO.StudentId);
                var course = await _courseRepository.GetByIdAsync(courseEnrollmentDTO.CourseId);

                if (student == null || course == null)
                {
                    _logger.LogWarning(
                        "Invalid student or course. Student ID: {StudentId}, Course ID: {CourseId}",
                        courseEnrollmentDTO.StudentId,
                        courseEnrollmentDTO.CourseId
                    );
                    throw new NotFoundException("Invalid student or course.");
                }

                if (course.CourseStudents.Any(cs => cs.StudentId == courseEnrollmentDTO.StudentId))
                {
                    _logger.LogWarning(
                        "Student with ID {StudentId} is already enrolled in course with ID {CourseId}",
                        courseEnrollmentDTO.StudentId,
                        courseEnrollmentDTO.CourseId
                    );
                    throw new InvalidOperationException("Student is already enrolled in this course.");
                }

                var courseStudent = new CourseStudent
                {
                    CourseId = courseEnrollmentDTO.CourseId,
                    StudentId = courseEnrollmentDTO.StudentId,
                    AssignedById = GetCurrentUserId(),
                    AssignedByName = GetCurrentUserName()
                };

                course.CourseStudents.Add(courseStudent);
                await _courseRepository.UpdateAsync(course);

                _logger.LogInformation(
                    "Successfully enrolled student with ID {StudentId} into course with ID {CourseId}",
                    courseEnrollmentDTO.StudentId,
                    courseEnrollmentDTO.CourseId
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to enroll student with ID {StudentId} into course with ID {CourseId}",
                    courseEnrollmentDTO.StudentId,
                    courseEnrollmentDTO.CourseId
                );
                throw; // Re-throw the exception after logging
            }
        }

        public async Task<IEnumerable<CourseDTO>> GetAllCoursesAsync()
        {
            _logger.LogInformation("Fetching all courses");

            try
            {
                var courses = await _courseRepository.GetAllAsync();

                var result = courses.Select(c => new CourseDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt,
                    CreatedBy = c.CreatedByName,
                }).ToList();

                _logger.LogInformation("Successfully fetched {CourseCount} courses", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch all courses");
                throw; // Re-throw the exception after logging
            }
        }

        public async Task<CourseDTO> GetCourseByIdAsync(Guid id)
        {
            // Log that the method was called with the CourseId
            _logger.LogInformation("GetCourseByIdAsync called with CourseId: {CourseId}", id);

            // Attempt to retrieve the course by its ID
            var course = await _courseRepository.GetByIdAsync(id);

            if (course == null)
            {
                // Log that the course was not found
                _logger.LogWarning("Course with Id {CourseId} not found.", id);

                // Throw an exception if the course is not found
                throw new NotFoundException($"Course with Id {id} not found.");
            }

            // Log that the course was found and include relevant details
            _logger.LogInformation("Course found: {CourseId}, Name: {CourseName}, CreatedAt: {CreatedAt}, CreatedById: {CreatedById}",
                course.Id, course.Name, course.CreatedAt, course.CreatedById);

            // Return the CourseDTO
            var courseDTO = new CourseDTO
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                CreatedAt = course.CreatedAt,
                CreatedBy = course.CreatedByName,
            };

            // Log that the CourseDTO is being returned with relevant details
            _logger.LogInformation("Returning CourseDTO: {CourseId}, {CourseName}, CreatedBy: {CreatedBy}",
                courseDTO.Id, courseDTO.Name, courseDTO.CreatedBy);

            return courseDTO;
        }

        public async Task<IEnumerable<ClassDTO>> GetClassesAsync(Guid courseId)
        {
            _logger.LogInformation("Fetching classes for course with ID {CourseId}", courseId);

            try
            {
                var course = await _courseRepository.GetByIdAsync(courseId);

                if (course == null)
                {
                    _logger.LogWarning("Course with ID {CourseId} not found", courseId);
                    throw new NotFoundException("Course not found.");
                }

                var classes = await _classRepository.GetByCourseIdAsync(courseId);

                var result = classes.Select(c => new ClassDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    CreatedBy = c.CreatedByName,
                    CreatedAt = c.CreatedAt,
                    Description = c.Description
                }).ToList();

                _logger.LogInformation("Successfully fetched {ClassCount} classes for course with ID {CourseId}", result.Count, courseId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch classes for course with ID {CourseId}", courseId);
                throw; // Re-throw the exception after logging
            }
        }

        public async Task<IEnumerable<StudentDTO>> GetStudentsAsync(Guid id)
        {
            _logger.LogInformation("Fetching students for course with ID {CourseId}", id);

            try
            {
                var course = await _courseRepository.GetByIdAsync(id);

                if (course == null)
                {
                    _logger.LogWarning("Course with ID {CourseId} not found", id);
                    throw new NotFoundException("Course not found.");
                }

                var courseStudents = course.CourseStudents;
                var enrolledStudents = courseStudents.Select(cs => cs.Student);

                var result = enrolledStudents.Select(s => new StudentDTO
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    DateOfBirth = s.DateOfBirth,
                    Email = s.Email
                }).ToList();

                _logger.LogInformation("Successfully fetched {StudentCount} students for course with ID {CourseId}", result.Count, id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch students for course with ID {CourseId}", id);
                throw; // Re-throw the exception after logging
            }
        }

        public async Task<CourseDTO> UpdateCourseAsync(UpdateCourseDTO updateCourseDTO)
        {
            var course = await _courseRepository.GetByIdAsync(updateCourseDTO.Id);
            if (course == null)
                throw new NotFoundException("Course not found.");

            course.Name = updateCourseDTO.Name;
            course.Description = updateCourseDTO.Description;

            await _courseRepository.UpdateAsync(course);

            return new CourseDTO
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                CreatedAt = course.CreatedAt,
                CreatedBy = course.CreatedByName,
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
