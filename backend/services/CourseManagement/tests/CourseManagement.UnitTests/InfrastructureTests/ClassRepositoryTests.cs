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
    public class ClassRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public ClassRepositoryTests()
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
        public async Task GetAllAsync_ShouldReturnAllClasses()
        {
            // Arrange
            using (var context = GetDbContext())
            {
                var repository = new ClassRepository(context);
                var testClass1 = new Class { Id = Guid.NewGuid(), Name = "Class 1", Description = "description", CreatedByName = string.Empty };
                var testClass2 = new Class { Id = Guid.NewGuid(), Name = "Class 2", Description = "description", CreatedByName = string.Empty };
                await context.Classes.AddRangeAsync(testClass1, testClass2);
                await context.SaveChangesAsync();

                // Act
                var result = await repository.GetAllAsync();

                // Assert
                Assert.Equal(2, result.Count());
                Assert.Contains(result, c => c.Name == "Class 1");
                Assert.Contains(result, c => c.Name == "Class 2");
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnClass_WhenClassExists()
        {
            // Arrange
            using (var context = GetDbContext())
            {
                var repository = new ClassRepository(context);
                var testClass = new Class { Id = Guid.NewGuid(), Name = "Class 1", Description = "description", CreatedByName = string.Empty };
                await context.Classes.AddAsync(testClass);
                await context.SaveChangesAsync();

                // Act
                var result = await repository.GetByIdAsync(testClass.Id);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(testClass.Name, result.Name);
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenClassDoesNotExist()
        {
            // Arrange
            using (var context = GetDbContext())
            {
                var repository = new ClassRepository(context);

                // Act
                var result = await repository.GetByIdAsync(Guid.NewGuid());

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task AddAsync_ShouldAddClass()
        {
            // Arrange
            using (var context = GetDbContext())
            {
                var repository = new ClassRepository(context);
                var testClass = new Class { Id = Guid.NewGuid(), Name = "Class 1", Description = "description", CreatedByName = string.Empty };

                // Act
                await repository.AddAsync(testClass);

                // Assert
                var result = await context.Classes.FindAsync(testClass.Id);
                Assert.NotNull(result);
                Assert.Equal(testClass.Name, result.Name);
            }
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateClass()
        {
            // Arrange
            using (var context = GetDbContext())
            {
                var repository = new ClassRepository(context);
                var testClass = new Class { Id = Guid.NewGuid(), Name = "Class 1", Description = "description", CreatedByName = string.Empty };
                await context.Classes.AddAsync(testClass);
                await context.SaveChangesAsync();

                testClass.Name = "Updated Class 1";

                // Act
                await repository.UpdateAsync(testClass);

                // Assert
                var result = await context.Classes.FindAsync(testClass.Id);
                Assert.NotNull(result);
                Assert.Equal("Updated Class 1", result.Name);
            }
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteClass()
        {
            // Arrange
            using (var context = GetDbContext())
            {
                var repository = new ClassRepository(context);
                var testClass = new Class { Id = Guid.NewGuid(), Name = "Class 1", Description = "description", CreatedByName = string.Empty };
                await context.Classes.AddAsync(testClass);
                await context.SaveChangesAsync();

                // Act
                await repository.DeleteAsync(testClass.Id);

                // Assert
                var result = await context.Classes.FindAsync(testClass.Id);
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetByCourseIdAsync_ShouldReturnClassesForCourse()
        {
            // Arrange
            using (var context = GetDbContext())
            {
                var repository = new ClassRepository(context);
                var courseId = Guid.NewGuid();
                var testClass1 = new Class { Id = Guid.NewGuid(), Name = "Class 1", Description = "description", CreatedByName = string.Empty };
                var testClass2 = new Class { Id = Guid.NewGuid(), Name = "Class 2", Description = "description", CreatedByName = string.Empty };
                testClass1.ClassCourses = new List<ClassCourse> { new ClassCourse { CourseId = courseId, AssignedByName = string.Empty } };
                testClass2.ClassCourses = new List<ClassCourse> { new ClassCourse { CourseId = Guid.NewGuid(), AssignedByName = string.Empty } };
                await context.Classes.AddRangeAsync(testClass1, testClass2);
                await context.SaveChangesAsync();

                // Act
                var result = await repository.GetByCourseIdAsync(courseId);

                // Assert
                Assert.Single(result);
                Assert.Equal(testClass1.Name, result.First().Class.Name);
            }
        }
    }
}