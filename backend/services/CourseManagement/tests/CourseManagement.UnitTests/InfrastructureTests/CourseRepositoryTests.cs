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
    public class CourseRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public CourseRepositoryTests()
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
        public async Task GetAllAsync_ShouldReturnAllCourses()
        {
            // Arrange
            using (var context = GetDbContext())
            {
                var repository = new CourseRepository(context);
                var testCourse1 = new Course { Id = Guid.NewGuid(), Name = "Course 1", Description = "Description 1", CreatedByName = string.Empty };
                var testCourse2 = new Course { Id = Guid.NewGuid(), Name = "Course 2", Description = "Description 2", CreatedByName = string.Empty };
                await context.Courses.AddRangeAsync(testCourse1, testCourse2);
                await context.SaveChangesAsync();

                // Act
                var result = await repository.GetAllAsync();

                // Assert
                Assert.Equal(2, result.Count());
                Assert.Contains(result, c => c.Name == "Course 1" && c.Description == "Description 1");
                Assert.Contains(result, c => c.Name == "Course 2" && c.Description == "Description 2");
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCourse_WhenCourseExists()
        {
            // Arrange
            using (var context = GetDbContext())
            {
                var repository = new CourseRepository(context);
                var testCourse = new Course { Id = Guid.NewGuid(), Name = "Course 1", Description = "Description 1" , CreatedByName = string.Empty };
                await context.Courses.AddAsync(testCourse);
                await context.SaveChangesAsync();

                // Act
                var result = await repository.GetByIdAsync(testCourse.Id);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(testCourse.Name, result.Name);
                Assert.Equal(testCourse.Description, result.Description);
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenCourseDoesNotExist()
        {
            // Arrange
            using (var context = GetDbContext())
            {
                var repository = new CourseRepository(context);

                // Act
                var result = await repository.GetByIdAsync(Guid.NewGuid());

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task AddAsync_ShouldAddCourse()
        {
            // Arrange
            using (var context = GetDbContext())
            {
                var repository = new CourseRepository(context);
                var testCourse = new Course { Id = Guid.NewGuid(), Name = "Course 1", Description = "Description 1", CreatedByName = string.Empty };

                // Act
                await repository.AddAsync(testCourse);

                // Assert
                var result = await context.Courses.FindAsync(testCourse.Id);
                Assert.NotNull(result);
                Assert.Equal(testCourse.Name, result.Name);
                Assert.Equal(testCourse.Description, result.Description);
            }
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateCourse()
        {
            // Arrange
            using (var context = GetDbContext())
            {
                var repository = new CourseRepository(context);
                var testCourse = new Course { Id = Guid.NewGuid(), Name = "Course 1", Description = "Description 1", CreatedByName = string.Empty };
                await context.Courses.AddAsync(testCourse);
                await context.SaveChangesAsync();

                testCourse.Name = "Updated Course 1";
                testCourse.Description = "Updated Description 1";

                // Act
                await repository.UpdateAsync(testCourse);

                // Assert
                var result = await context.Courses.FindAsync(testCourse.Id);
                Assert.NotNull(result);
                Assert.Equal("Updated Course 1", result.Name);
                Assert.Equal("Updated Description 1", result.Description);
            }
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteCourse()
        {
            // Arrange
            using (var context = GetDbContext())
            {
                var repository = new CourseRepository(context);
                var testCourse = new Course { Id = Guid.NewGuid(), Name = "Course 1", Description = "Description 1", CreatedByName = string.Empty };
                await context.Courses.AddAsync(testCourse);
                await context.SaveChangesAsync();

                // Act
                await repository.DeleteAsync(testCourse.Id);

                // Assert
                var result = await context.Courses.FindAsync(testCourse.Id);
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetByClassIdAsync_ShouldReturnCoursesForClass()
        {
            // Arrange
            using (var context = GetDbContext())
            {
                var repository = new CourseRepository(context);
                var classId = Guid.NewGuid();
                var testCourse1 = new Course { Id = Guid.NewGuid(), Name = "Course 1", Description = "Description 1", CreatedByName = string.Empty };
                var testCourse2 = new Course { Id = Guid.NewGuid(), Name = "Course 2", Description = "Description 2", CreatedByName = string.Empty };
                testCourse1.ClassCourses = new List<ClassCourse> { new ClassCourse { ClassId = classId, AssignedByName = string.Empty } };
                testCourse2.ClassCourses = new List<ClassCourse> { new ClassCourse { ClassId = Guid.NewGuid(), AssignedByName = string.Empty } };
                await context.Courses.AddRangeAsync(testCourse1, testCourse2);
                await context.SaveChangesAsync();

                // Act
                var result = await repository.GetByClassIdAsync(classId);

                // Assert
                Assert.Single(result);
                Assert.Equal(testCourse1.Name, result.First().Name);
                Assert.Equal(testCourse1.Description, result.First().Description);
            }
        }
    }
}