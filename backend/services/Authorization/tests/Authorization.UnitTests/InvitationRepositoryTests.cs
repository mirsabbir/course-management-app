using Authorization.Infrastructure.Repositories;
using Authorization.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Authorization.Infrastructure;

namespace Authorization.UnitTests
{
    public class InvitationRepositoryTests
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
        public async Task SaveInvitationAsync_ShouldSaveInvitation()
        {
            // Arrange
            var dbContext = await GetDbContextAsync();
            var repository = new InvitationRepository(dbContext);
            var invitation = new Invitation
            {
                Id = Guid.NewGuid(),
                Token = "test-token",
                FullName = "test full name",
                Email = "test@test.com",
            };

            // Act
            await repository.SaveInvitationAsync(invitation);
            var savedInvitation = await dbContext.Invitations.FirstOrDefaultAsync(i => i.Token == "test-token");

            // Assert
            savedInvitation.Should().NotBeNull();
            savedInvitation!.Token.Should().Be("test-token");
        }

        [Fact]
        public async Task GetInvitationByTokenAsync_ShouldReturnInvitation_WhenTokenExists()
        {
            // Arrange
            var dbContext = await GetDbContextAsync();
            var repository = new InvitationRepository(dbContext);
            var invitation = new Invitation
            {
                Id = Guid.NewGuid(),
                Token = "existing-token",
                FullName = "test full name",
                Email = "test@test.com",
            };

            dbContext.Invitations.Add(invitation);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await repository.GetInvitationByTokenAsync("existing-token");

            // Assert
            result.Should().NotBeNull();
            result!.Token.Should().Be("existing-token");
        }

        [Fact]
        public async Task GetInvitationByTokenAsync_ShouldReturnNull_WhenTokenDoesNotExist()
        {
            // Arrange
            var dbContext = await GetDbContextAsync();
            var repository = new InvitationRepository(dbContext);

            // Act
            var result = await repository.GetInvitationByTokenAsync("non-existent-token");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteInvitationAsync_ShouldDeleteInvitation_WhenTokenExists()
        {
            // Arrange
            var dbContext = await GetDbContextAsync();
            var repository = new InvitationRepository(dbContext);
            var invitation = new Invitation
            {
                Id = Guid.NewGuid(),
                Token = "delete-token",
                FullName = "test full name",
                Email = "test@test.com",
            };

            dbContext.Invitations.Add(invitation);
            await dbContext.SaveChangesAsync();

            // Act
            await repository.DeleteInvitationAsync("delete-token");
            var deletedInvitation = await dbContext.Invitations.FirstOrDefaultAsync(i => i.Token == "delete-token");

            // Assert
            deletedInvitation.Should().BeNull();
        }

        [Fact]
        public async Task DeleteInvitationAsync_ShouldNotFail_WhenTokenDoesNotExist()
        {
            // Arrange
            var dbContext = await GetDbContextAsync();
            var repository = new InvitationRepository(dbContext);

            // Act
            Func<Task> act = async () => await repository.DeleteInvitationAsync("non-existent-token");

            // Assert
            await act.Should().NotThrowAsync();
        }
    }
}
