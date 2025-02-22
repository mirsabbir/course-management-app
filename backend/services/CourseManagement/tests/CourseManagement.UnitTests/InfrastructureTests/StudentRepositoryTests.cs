using CourseManagement.Application.Interfaces;
using CourseManagement.Domain;
using CourseManagement.Infrastructure;
using CourseManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CourseManagement.UnitTests.InfrastructureTests
{
    public class StudentRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public StudentRepositoryTests()
        {
            // Set up an in-memory database for testing
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique name for each test
                .Options;
        }

        private ApplicationDbContext GetDbContext()
        {
            return new ApplicationDbContext(_dbContextOptions);
        }

        [Fact]
        public async Task GetAllStudentsAsync_ShouldReturnAllStudents()
        {
            // Arrange
            using (var context = GetDbContext())
            {
                var repository = new StudentRepository(context);
                var testStudent1 = new Student
                {
                    Id = Guid.NewGuid(),
                    FullName = "John Doe",
                    Email = "john.doe@example.com",
                    DateOfBirth = new DateTime(2000, 1, 1),
                    UserId = Guid.NewGuid()
                };
                var testStudent2 = new Student
                {
                    Id = Guid.NewGuid(),
                    FullName = "Jane Smith",
                    Email = "jane.smith@example.com",
                    DateOfBirth = new DateTime(1999, 5, 15),
                    UserId = Guid.NewGuid()
                };
                await context.Students.AddRangeAsync(testStudent1, testStudent2);
                await context.SaveChangesAsync();

                // Act
                var result = await repository.GetAllStudentsAsync();

                // Assert
                Assert.Equal(2, result.Count());
                Assert.Contains(result, s => s.FullName == "John Doe" && s.Email == "john.doe@example.com");
                Assert.Contains(result, s => s.FullName == "Jane Smith" && s.Email == "jane.smith@example.com");
            }
        }

        [Fact]
        public async Task GetStudentByIdAsync_ShouldReturnStudent_WhenStudentExists()
        {
            // Arrange
            using (var context = GetDbContext())
            {
                var repository = new StudentRepository(context);
                var testStudent = new Student
                {
                    Id = Guid.NewGuid(),
                    FullName = "John Doe",
                    Email = "john.doe@example.com",
                    DateOfBirth = new DateTime(2000, 1, 1),
                    UserId = Guid.NewGuid()
                };
                await context.Students.AddAsync(testStudent);
                await context.SaveChangesAsync();

                // Act
                var result = await repository.GetStudentByIdAsync(testStudent.Id);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(testStudent.FullName, result.FullName);
                Assert.Equal(testStudent.Email, result.Email);
                Assert.Equal(testStudent.DateOfBirth, result.DateOfBirth);
                Assert.Equal(testStudent.UserId, result.UserId);
            }
        }

        [Fact]
        public async Task GetStudentByIdAsync_ShouldReturnNull_WhenStudentDoesNotExist()
        {
            // Arrange
            using (var context = GetDbContext())
            {
                var repository = new StudentRepository(context);

                // Act
                var result = await repository.GetStudentByIdAsync(Guid.NewGuid());

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task AddStudentAsync_ShouldAddStudent()
        {
            // Arrange
            using (var context = GetDbContext())
            {
                var repository = new StudentRepository(context);
                var testStudent = new Student
                {
                    Id = Guid.NewGuid(),
                    FullName = "John Doe",
                    Email = "john.doe@example.com",
                    DateOfBirth = new DateTime(2000, 1, 1),
                    UserId = Guid.NewGuid()
                };

                // Act
                await repository.AddStudentAsync(testStudent);

                // Assert
                var result = await context.Students.FindAsync(testStudent.Id);
                Assert.NotNull(result);
                Assert.Equal(testStudent.FullName, result.FullName);
                Assert.Equal(testStudent.Email, result.Email);
                Assert.Equal(testStudent.DateOfBirth, result.DateOfBirth);
                Assert.Equal(testStudent.UserId, result.UserId);
            }
        }

        [Fact]
        public async Task UpdateStudentAsync_ShouldUpdateStudent()
        {
            // Arrange
            using (var context = GetDbContext())
            {
                var repository = new StudentRepository(context);
                var testStudent = new Student
                {
                    Id = Guid.NewGuid(),
                    FullName = "John Doe",
                    Email = "john.doe@example.com",
                    DateOfBirth = new DateTime(2000, 1, 1),
                    UserId = Guid.NewGuid()
                };
                await context.Students.AddAsync(testStudent);
                await context.SaveChangesAsync();

                testStudent.FullName = "Updated John Doe";
                testStudent.Email = "updated.john.doe@example.com";
                testStudent.DateOfBirth = new DateTime(2001, 2, 2);

                // Act
                await repository.UpdateStudentAsync(testStudent);

                // Assert
                var result = await context.Students.FindAsync(testStudent.Id);
                Assert.NotNull(result);
                Assert.Equal("Updated John Doe", result.FullName);
                Assert.Equal("updated.john.doe@example.com", result.Email);
                Assert.Equal(new DateTime(2001, 2, 2), result.DateOfBirth);
            }
        }

        [Fact]
        public async Task DeleteStudentAsync_ShouldDeleteStudent()
        {
            // Arrange
            using (var context = GetDbContext())
            {
                var repository = new StudentRepository(context);
                var testStudent = new Student
                {
                    Id = Guid.NewGuid(),
                    FullName = "John Doe",
                    Email = "john.doe@example.com",
                    DateOfBirth = new DateTime(2000, 1, 1),
                    UserId = Guid.NewGuid()
                };
                await context.Students.AddAsync(testStudent);
                await context.SaveChangesAsync();

                // Act
                await repository.DeleteStudentAsync(testStudent.Id);

                // Assert
                var result = await context.Students.FindAsync(testStudent.Id);
                Assert.Null(result);
            }
        }
    }
}