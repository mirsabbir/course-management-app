using Authorization.Infrastructure.Repositories;
using Authorization.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Authorization.Infrastructure;

namespace Authorization.UnitTests
{
    public class UserRepositoryTests
    {
        private async Task<ApplicationDbContext> GetDbContextAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var dbContext = new ApplicationDbContext(options);
            await dbContext.Database.EnsureCreatedAsync();

            return dbContext;
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var dbContext = await GetDbContextAsync();
            var repository = new UserRepository(dbContext);
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid().ToString(), FullName = "Alice" },
                new User { Id = Guid.NewGuid().ToString(), FullName = "Bob" }
            };

            dbContext.Users.AddRange(users);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await repository.GetAllUsersAsync();

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain(u => u.FullName == "Alice");
            result.Should().Contain(u => u.FullName == "Bob");
            result.Should().Contain(u => u.Email == "staff@staff.com"); // seed user
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var dbContext = await GetDbContextAsync();
            var repository = new UserRepository(dbContext);
            var userId = Guid.NewGuid().ToString();
            var user = new User { Id = userId, FullName = "John Doe" };

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await repository.GetUserByIdAsync(Guid.Parse(userId));

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(userId);
            result.FullName.Should().Be("John Doe");
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var dbContext = await GetDbContextAsync();
            var repository = new UserRepository(dbContext);
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await repository.GetUserByIdAsync(nonExistentId);

            // Assert
            result.Should().BeNull();
        }
    }
}
