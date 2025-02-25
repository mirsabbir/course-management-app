using Castle.Core.Logging;
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
    public class ClassServiceTests
    {
        private readonly Mock<IClassRepository> _mockClassRepository;
        private readonly Mock<ICourseRepository> _mockCourseRepository;
        private readonly Mock<IStudentRepository> _mockStudentRepository;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ILogger<ClassService>> _mockLogger;
        private readonly ClassService _classService;

        public ClassServiceTests()
        {
            _mockClassRepository = new Mock<IClassRepository>();
            _mockCourseRepository = new Mock<ICourseRepository>();
            _mockStudentRepository = new Mock<IStudentRepository>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<ClassService>>();

            _classService = new ClassService(
                _mockClassRepository.Object,
                _mockCourseRepository.Object,
                _mockStudentRepository.Object,
                _mockHttpContextAccessor.Object,
                _mockUserService.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task CreateClassAsync_ShouldCreateClass()
        {
            // Arrange
            var createClassDTO = new CreateClassDTO
            {
                Name = "Math 101",
                Description = "Introduction to Mathematics"
            };

            var userId = Guid.NewGuid();
            var userName = "John Doe";

            SetupHttpContext(userId.ToString(), userName);

            _mockClassRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Class>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _classService.CreateClassAsync(createClassDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createClassDTO.Name, result.Name);
            Assert.Equal(createClassDTO.Description, result.Description);
            Assert.Equal(userName, result.CreatedBy);
            _mockClassRepository.Verify(repo => repo.AddAsync(It.IsAny<Class>()), Times.Once);
        }

        [Fact]
        public async Task DeleteClassAsync_ShouldDeleteClass_WhenClassExists()
        {
            // Arrange
            var classId = Guid.NewGuid();

            _mockClassRepository
                .Setup(repo => repo.GetByIdAsync(classId))
                .ReturnsAsync(new Class { Id = classId, CreatedByName = string.Empty, Description = string.Empty, Name = "Class" }); // Simulate that the class exists

            _mockClassRepository
                .Setup(repo => repo.DeleteAsync(classId))
                .Returns(Task.CompletedTask);

            // Act
            await _classService.DeleteClassAsync(classId);

            // Assert
            _mockClassRepository.Verify(repo => repo.GetByIdAsync(classId), Times.Once);
            _mockClassRepository.Verify(repo => repo.DeleteAsync(classId), Times.Once);
        }

        [Fact]
        public async Task DeleteClassAsync_ShouldThrowNotFoundException_WhenClassDoesNotExist()
        {
            // Arrange
            var classId = Guid.NewGuid();

            _mockClassRepository
                .Setup(repo => repo.GetByIdAsync(classId))
                .ReturnsAsync((Class)null); // Simulate that the class does not exist

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _classService.DeleteClassAsync(classId));

            _mockClassRepository.Verify(repo => repo.GetByIdAsync(classId), Times.Once);
            _mockClassRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>()), Times.Never); // Ensure delete is never called
        }


        [Fact]
        public async Task EnrollStudentAsync_ShouldEnrollStudent()
        {
            // Arrange
            var classEnrollmentDTO = new ClassEnrollmentDTO
            {
                ClassId = Guid.NewGuid(),
                StudentId = Guid.NewGuid()
            };

            var student = new Student { Id = classEnrollmentDTO.StudentId, CreatedByName = "Test user", Email = "student@student.com", FullName = "Student" };
            var classEntity = new Class
            {
                Id = classEnrollmentDTO.ClassId,
                ClassStudents = new List<ClassStudent>(),
                CreatedByName = "test staff",
                Description = "Description",
                Name = "Math 104"
            };

            _mockStudentRepository
                .Setup(repo => repo.GetStudentByIdAsync(classEnrollmentDTO.StudentId))
                .ReturnsAsync(student);

            _mockClassRepository
                .Setup(repo => repo.GetByIdAsync(classEnrollmentDTO.ClassId))
                .ReturnsAsync(classEntity);

            _mockClassRepository
                .Setup(repo => repo.UpdateAsync(classEntity))
                .Returns(Task.CompletedTask);

            // Act
            await _classService.EnrollStudentAsync(classEnrollmentDTO);

            // Assert
            _mockClassRepository.Verify(repo => repo.UpdateAsync(classEntity), Times.Once);
            Assert.Single(classEntity.ClassStudents);
            Assert.Equal(classEnrollmentDTO.StudentId, classEntity.ClassStudents.First().StudentId);
        }

        [Fact]
        public async Task EnrollStudentAsync_ShouldThrowException_WhenStudentOrClassNotFound()
        {
            // Arrange
            var classEnrollmentDTO = new ClassEnrollmentDTO
            {
                ClassId = Guid.NewGuid(),
                StudentId = Guid.NewGuid()
            };

            _mockStudentRepository
                .Setup(repo => repo.GetStudentByIdAsync(classEnrollmentDTO.StudentId))
                .ReturnsAsync((Student)null);

            _mockClassRepository
                .Setup(repo => repo.GetByIdAsync(classEnrollmentDTO.ClassId))
                .ReturnsAsync((Class)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _classService.EnrollStudentAsync(classEnrollmentDTO));
        }

        [Fact]
        public async Task GetAllClassesAsync_ShouldReturnAllClasses()
        {
            // Arrange
            var classes = new List<Class>
            {
                new Class { Id = Guid.NewGuid(), Name = "Math 101", CreatedById = Guid.NewGuid(), CreatedByName = "John Doe", Description = string.Empty },
                new Class {Id = Guid.NewGuid(), Name = "Science 101", CreatedById = Guid.NewGuid(), CreatedByName = "Jane Smith", Description = string.Empty}
            };

            _mockClassRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(classes);

            // Act
            var result = await _classService.GetAllClassesAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Name == "Math 101" && c.CreatedBy == "John Doe");
            Assert.Contains(result, c => c.Name == "Science 101" && c.CreatedBy == "Jane Smith");
        }

        [Fact]
        public async Task GetClassByIdAsync_ShouldReturnClass()
        {
            // Arrange
            var classId = Guid.NewGuid();
            var classEntity = new Class
            {
                Id = classId,
                Name = "Math 101",
                CreatedById = Guid.NewGuid(),
                CreatedByName = "John Doe",
                Description = string.Empty,
            };

            _mockClassRepository
                .Setup(repo => repo.GetByIdAsync(classId))
                .ReturnsAsync(classEntity);


            // Act
            var result = await _classService.GetClassByIdAsync(classId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(classEntity.Name, result.Name);
            Assert.Equal("John Doe", result.CreatedBy);
        }

        [Fact]
        public async Task GetClassByIdAsync_ShouldThrowException_WhenClassNotFound()
        {
            // Arrange
            var classId = Guid.NewGuid();

            _mockClassRepository
                .Setup(repo => repo.GetByIdAsync(classId))
                .ReturnsAsync((Class)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _classService.GetClassByIdAsync(classId));
        }

        private void SetupHttpContext(string userId, string userName)
        {
            var claims = new List<Claim>
            {
                new Claim("userId", userId),
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