using CourseManagement.Application.DTOs;
using CourseManagement.Application.DTOs.Classes;
using CourseManagement.Application.DTOs.Courses;
using CourseManagement.Application.DTOs.Enrollment;
using CourseManagement.Application.DTOs.Students;
using CourseManagement.Application.Exceptions;
using CourseManagement.Application.Interfaces;
using CourseManagement.Application.Services;
using CourseManagement.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace CourseManagement.UnitTests.ApplicationTests
{
    public class CourseServiceTests
    {
        private readonly Mock<ICourseRepository> _mockCourseRepository;
        private readonly Mock<IClassRepository> _mockClassRepository;
        private readonly Mock<IStudentRepository> _mockStudentRepository;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<ILogger<CourseService>> _mockLogger;
        private readonly Mock<ICourseStudentRepository> _mockCourseStudentRepository;
        private readonly Mock<IClassCourseRepository> _mockClassCourseRepository;
        private readonly CourseService _courseService;

        public CourseServiceTests()
        {
            _mockCourseRepository = new Mock<ICourseRepository>();
            _mockClassRepository = new Mock<IClassRepository>();
            _mockStudentRepository = new Mock<IStudentRepository>();
            _mockUserService = new Mock<IUserService>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockCourseStudentRepository = new Mock<ICourseStudentRepository>();
            _mockClassCourseRepository = new Mock<IClassCourseRepository>();
            _mockLogger = new Mock<ILogger<CourseService>>();

            _courseService = new CourseService(
                _mockCourseRepository.Object,
                _mockClassRepository.Object,
                _mockStudentRepository.Object,
                _mockUserService.Object,
                _mockHttpContextAccessor.Object,
                _mockCourseStudentRepository.Object,
                _mockClassCourseRepository.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task CreateCourseAsync_ShouldCreateCourse()
        {
            // Arrange
            var createCourseDTO = new CreateCourseDTO
            {
                Name = "Math 101",
                Description = "Introduction to Mathematics"
            };

            var userId = Guid.NewGuid();
            var userName = "John Doe";

            SetupHttpContext(userId.ToString(), userName);

            _mockCourseRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Course>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _courseService.CreateCourseAsync(createCourseDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createCourseDTO.Name, result.Name);
            Assert.Equal(createCourseDTO.Description, result.Description);
            Assert.Equal(userName, result.CreatedBy);
            _mockCourseRepository.Verify(repo => repo.AddAsync(It.IsAny<Course>()), Times.Once);
        }

        [Fact]
        public async Task DeleteCourseAsync_ShouldDeleteCourse()
        {
            // Arrange
            var courseId = Guid.NewGuid();

            _mockCourseRepository
                .Setup(repo => repo.GetByIdAsync(courseId))
                .ReturnsAsync(new Course { Id = courseId, CreatedByName = string.Empty, Description = string.Empty, Name = string.Empty });

            _mockCourseRepository
                .Setup(repo => repo.DeleteAsync(courseId))
                .Returns(Task.CompletedTask);

            // Act
            await _courseService.DeleteCourseAsync(courseId);

            // Assert
            _mockCourseRepository.Verify(repo => repo.GetByIdAsync(courseId), Times.Once);
            _mockCourseRepository.Verify(repo => repo.DeleteAsync(courseId), Times.Once);
        }

        [Fact]
        public async Task DeleteCourseAsync_ShouldThrowNotFoundException_WhenCourseDoesNotExist()
        {
            // Arrange
            var courseId = Guid.NewGuid();

            _mockCourseRepository
                .Setup(repo => repo.GetByIdAsync(courseId))
                .ReturnsAsync((Course)null); // Simulate course not found

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _courseService.DeleteCourseAsync(courseId));

            _mockCourseRepository.Verify(repo => repo.GetByIdAsync(courseId), Times.Once);
            _mockCourseRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }


        [Fact]
        public async Task EnrollStudentAsync_ShouldEnrollStudent_WhenStudentAndCourseExistAndNotAlreadyEnrolled()
        {
            // Arrange
            var courseEnrollmentDTO = new CourseEnrollmentDTO
            {
                StudentId = Guid.NewGuid(),
                CourseId = Guid.NewGuid()
            };

            var student = new Student {Id = courseEnrollmentDTO.StudentId, CreatedByName = "test", Email = "a@a.com", FullName = "full name" };
            var course = new Course {Id = courseEnrollmentDTO.CourseId, CreatedByName = "test", Name = "course 1", Description = "123" };

            _mockStudentRepository
                .Setup(repo => repo.GetStudentByIdAsync(courseEnrollmentDTO.StudentId))
                .ReturnsAsync(student);

            _mockCourseRepository
                .Setup(repo => repo.GetByIdAsync(courseEnrollmentDTO.CourseId))
                .ReturnsAsync(course);

            _mockCourseStudentRepository
                .Setup(repo => repo.ExistsAsync(course.Id, student.Id))
                .ReturnsAsync(false);

            _mockCourseStudentRepository
                .Setup(repo => repo.AddAsync(It.IsAny<CourseStudent>()))
                .Returns(Task.CompletedTask);

            // Act
            await _courseService.EnrollStudentAsync(courseEnrollmentDTO);

            // Assert
            _mockCourseStudentRepository.Verify(repo => repo.AddAsync(It.IsAny<CourseStudent>()), Times.Once);
        }

        [Fact]
        public async Task EnrollStudentAsync_ShouldThrowNotFoundException_WhenStudentNotFound()
        {
            // Arrange
            var courseEnrollmentDTO = new CourseEnrollmentDTO
            {
                StudentId = Guid.NewGuid(),
                CourseId = Guid.NewGuid()
            };

            _mockStudentRepository
                .Setup(repo => repo.GetStudentByIdAsync(courseEnrollmentDTO.StudentId))
                .ReturnsAsync((Student)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _courseService.EnrollStudentAsync(courseEnrollmentDTO));
        }

        [Fact]
        public async Task EnrollStudentAsync_ShouldThrowNotFoundException_WhenCourseNotFound()
        {
            // Arrange
            var courseEnrollmentDTO = new CourseEnrollmentDTO
            {
                StudentId = Guid.NewGuid(),
                CourseId = Guid.NewGuid()
            };

            var student = new Student { Id = courseEnrollmentDTO.StudentId, CreatedByName = "test", Email = "a@a.com", FullName = "full name" };

            _mockStudentRepository
                .Setup(repo => repo.GetStudentByIdAsync(courseEnrollmentDTO.StudentId))
                .ReturnsAsync(student);

            _mockCourseRepository
                .Setup(repo => repo.GetByIdAsync(courseEnrollmentDTO.CourseId))
                .ReturnsAsync((Course)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _courseService.EnrollStudentAsync(courseEnrollmentDTO));
        }

        [Fact]
        public async Task EnrollStudentAsync_ShouldThrowInvalidOperationException_WhenStudentAlreadyEnrolled()
        {
            // Arrange
            var courseEnrollmentDTO = new CourseEnrollmentDTO
            {
                StudentId = Guid.NewGuid(),
                CourseId = Guid.NewGuid()
            };

            var student = new Student { Id = courseEnrollmentDTO.StudentId, CreatedByName = "test", Email = "a@a.com", FullName = "full name" };
            var course = new Course { Id = courseEnrollmentDTO.CourseId, CreatedByName = "test", Name = "course 1", Description = "123" };

            _mockStudentRepository
                .Setup(repo => repo.GetStudentByIdAsync(courseEnrollmentDTO.StudentId))
                .ReturnsAsync(student);

            _mockCourseRepository
                .Setup(repo => repo.GetByIdAsync(courseEnrollmentDTO.CourseId))
                .ReturnsAsync(course);

            _mockCourseStudentRepository
                .Setup(repo => repo.ExistsAsync(course.Id, student.Id))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _courseService.EnrollStudentAsync(courseEnrollmentDTO));
        }

        [Fact]
        public async Task EnrollStudentAsync_ShouldLogAndRethrowException_WhenUnexpectedErrorOccurs()
        {
            // Arrange
            var courseEnrollmentDTO = new CourseEnrollmentDTO
            {
                StudentId = Guid.NewGuid(),
                CourseId = Guid.NewGuid()
            };

            var student = new Student { Id = courseEnrollmentDTO.StudentId, CreatedByName = "test", Email = "a@a.com", FullName = "full name" };
            var course = new Course { Id = courseEnrollmentDTO.CourseId, CreatedByName = "test", Name = "course 1", Description = "123" };

            _mockStudentRepository
                .Setup(repo => repo.GetStudentByIdAsync(courseEnrollmentDTO.StudentId))
                .ReturnsAsync(student);

            _mockCourseRepository
                .Setup(repo => repo.GetByIdAsync(courseEnrollmentDTO.CourseId))
                .ReturnsAsync(course);

            _mockCourseStudentRepository
                .Setup(repo => repo.ExistsAsync(course.Id, student.Id))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _courseService.EnrollStudentAsync(courseEnrollmentDTO));
        }

        [Fact]
        public async Task EnrollStudentAsync_ShouldThrowException_WhenStudentOrCourseNotFound()
        {
            // Arrange
            var courseEnrollmentDTO = new CourseEnrollmentDTO
            {
                CourseId = Guid.NewGuid(),
                StudentId = Guid.NewGuid()
            };

            _mockStudentRepository
                .Setup(repo => repo.GetStudentByIdAsync(courseEnrollmentDTO.StudentId))
                .ReturnsAsync((Student)null);

            _mockCourseRepository
                .Setup(repo => repo.GetByIdAsync(courseEnrollmentDTO.CourseId))
                .ReturnsAsync((Course)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _courseService.EnrollStudentAsync(courseEnrollmentDTO));
        }

        [Fact]
        public async Task GetAllCoursesAsync_ShouldReturnAllCourses()
        {
            // Arrange
            var courses = new List<Course>
            {
                new Course { Id = Guid.NewGuid(), Name = "Math 101", CreatedById = Guid.NewGuid(), CreatedByName = "John Doe", Description = string.Empty },
                new Course {Id = Guid.NewGuid(), Name = "Science 101", CreatedById = Guid.NewGuid(), CreatedByName = "Jane Smith", Description = string.Empty}
            };

            _mockCourseRepository
                .Setup(repo => repo.CountAsync())
                .ReturnsAsync(2);

            _mockCourseRepository
                .Setup(repo => repo.GetPagedAsync(1, 10))
                .ReturnsAsync(courses);

            // Act
            var result = await _courseService.GetAllCoursesAsync();

            // Assert
            Assert.Equal(2, result.TotalCount);
            Assert.Contains(result.Courses, c => c.Name == "Math 101" && c.CreatedBy == "John Doe");
            Assert.Contains(result.Courses, c => c.Name == "Science 101" && c.CreatedBy == "Jane Smith");
        }

        [Fact]
        public async Task GetCourseByIdAsync_ShouldReturnCourse()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new Course
            {
                Id = courseId,
                Name = "Math 101",
                CreatedById = Guid.NewGuid(),
                CreatedByName = "John Doe",
                Description = string.Empty
            };

            _mockCourseRepository
                .Setup(repo => repo.GetByIdAsync(courseId))
                .ReturnsAsync(course);

            // Act
            var result = await _courseService.GetCourseByIdAsync(courseId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(course.Name, result.Name);
            Assert.Equal("John Doe", result.CreatedBy);
        }

        [Fact]
        public async Task GetCourseByIdAsync_ShouldThrowException_WhenCourseNotFound()
        {
            // Arrange
            var courseId = Guid.NewGuid();

            _mockCourseRepository
                .Setup(repo => repo.GetByIdAsync(courseId))
                .ReturnsAsync((Course)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _courseService.GetCourseByIdAsync(courseId));
        }

        [Fact]
        public async Task GetClassesAsync_ShouldReturnClassesForCourse()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new Course { Id = courseId, CreatedByName = string.Empty, Description = "description", Name = "New Course" };

            var classes = new List<Class>
            {
                new Class { Id = Guid.NewGuid(), Name = "Class 1", CreatedById = Guid.NewGuid(), CreatedByName = "John Doe", Description = string.Empty },
                new Class {Id = Guid.NewGuid(), Name = "Class 2", CreatedById = Guid.NewGuid(), CreatedByName = "Jane Smith", Description = string.Empty}
            };

            var classCourses = new List<ClassCourse>
            {
                new ClassCourse
                {
                    AssignedAt = DateTime.UtcNow,
                    AssignedByName = string.Empty,
                    AssignedById = Guid.NewGuid(),
                    ClassId = classes[0].Id,
                    Class = classes[0],
                    Course = course,
                    CourseId = course.Id,
                },
                new ClassCourse
                {
                    AssignedAt = DateTime.UtcNow,
                    AssignedByName = string.Empty,
                    AssignedById = Guid.NewGuid(),
                    ClassId = classes[1].Id,
                    Class = classes[1],
                    Course = course,
                    CourseId = course.Id,
                }
            };

            _mockCourseRepository
                .Setup(repo => repo.GetByIdAsync(courseId))
                .ReturnsAsync(course);

            _mockClassRepository
                .Setup(repo => repo.GetByCourseIdAsync(courseId))
                .ReturnsAsync(classCourses);

            // Act
            var result = await _courseService.GetClassesAsync(courseId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Name == "Class 1" && c.CreatedBy == "John Doe");
            Assert.Contains(result, c => c.Name == "Class 2" && c.CreatedBy == "Jane Smith");
        }

        [Fact]
        public async Task GetStudentsAsync_ShouldReturnStudentsForCourse()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new Course
            {
                Id = courseId,
                CreatedByName = string.Empty,
                Description = string.Empty,
                Name = "New Course"
            };

            var students = new List<Student>
                {
                    new Student { Id = Guid.NewGuid(), FullName = "John Doe", Email = "john@example.com", DateOfBirth = new DateTime(2000, 1, 1), CreatedByName = string.Empty  },
                    new Student { Id = Guid.NewGuid(), FullName = "Jane Smith", Email = "jane@example.com", DateOfBirth = new DateTime(1999, 5, 15), CreatedByName = string.Empty }
                };

            var courseStudents = new List<CourseStudent> {
            new CourseStudent
                {
                    AssignedByName = "Assigned by",
                    AssignedAt = DateTime.UtcNow,
                    Student = students[0],
                    AssignedById = Guid.NewGuid(),
                    CourseId = course.Id,
                    StudentId = students[0].Id,
                },
            new CourseStudent
                {
                    AssignedByName = "Assigned by",
                    AssignedAt = DateTime.UtcNow,
                    Student = students[1],
                    AssignedById = Guid.NewGuid(),
                    CourseId = course.Id,
                    StudentId = students[1].Id,
                },
            };

            _mockCourseRepository
                .Setup(repo => repo.GetByIdAsync(courseId))
                .ReturnsAsync(course);

            _mockCourseStudentRepository
               .Setup(repo => repo.GetStudentsByCourseIdAsync(courseId))
               .ReturnsAsync(courseStudents);

            // Act
            var result = await _courseService.GetStudentsAsync(courseId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, s => s.FullName == "John Doe" && s.Email == "john@example.com");
            Assert.Contains(result, s => s.FullName == "Jane Smith" && s.Email == "jane@example.com");
        }

        [Fact]
        public async Task UpdateCourseAsync_ShouldUpdateCourse()
        {
            // Arrange
            var updateCourseDTO = new UpdateCourseDTO
            {
                Id = Guid.NewGuid(),
                Name = "Updated Math 101",
                Description = "Updated Description"
            };

            var course = new Course
            {
                Id = updateCourseDTO.Id,
                Name = "Math 101",
                Description = "Introduction to Mathematics",
                CreatedById = Guid.NewGuid(),
                CreatedByName = "John Doe"
            };

            _mockCourseRepository
                .Setup(repo => repo.GetByIdAsync(updateCourseDTO.Id))
                .ReturnsAsync(course);

            _mockCourseRepository
                .Setup(repo => repo.UpdateAsync(course))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _courseService.UpdateCourseAsync(updateCourseDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateCourseDTO.Name, result.Name);
            Assert.Equal(updateCourseDTO.Description, result.Description);
            Assert.Equal("John Doe", result.CreatedBy);
            _mockCourseRepository.Verify(repo => repo.UpdateAsync(course), Times.Once);
        }

        private void SetupHttpContext(string userId, string userName)
        {
            var claims = new List<Claim>
            {
                new Claim("UserId", userId),
                new Claim("fullName", userName)
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            };

            _mockHttpContextAccessor
                .Setup(accessor => accessor.HttpContext)
                .Returns(httpContext);
        }
    }
}