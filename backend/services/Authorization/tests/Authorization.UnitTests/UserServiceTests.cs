using Moq;
using Microsoft.AspNetCore.Identity;
using Authorization.Domain;
using Authorization.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Authorization.Application.Interfaces;
using Authorization.Application.Services;

namespace Authorization.UnitTests
{
    public class UserServiceTests
    {
        private readonly Mock<IInvitationRepository> _mockInvitationRepo;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly UserService _userService;
        private readonly Mock<UserManager<User>> _mockUserManager;

        private List<User> _users;

        public UserServiceTests()
        {
            _mockInvitationRepo = new Mock<IInvitationRepository>();
            _mockEmailService = new Mock<IEmailService>();

            // Set up a list of users for testing
            _users = new List<User>
            {
                new User { UserName = "user1@example.com", Email = "user1@example.com", FullName = "User One" },
                new User { UserName = "user2@example.com", Email = "user2@example.com", FullName = "User Two" }
            };

            // Mocking the UserManager<User> using the helper method
            _mockUserManager = MockUserManager(_users);

            // Initialize the UserService with the mocked dependencies
            _mockUserRepo = new Mock<IUserRepository>();
            _userService = new UserService(
                _mockInvitationRepo.Object,
                _mockEmailService.Object,
                _mockUserManager.Object,
                _mockUserRepo.Object);
        }

        // Helper method to mock UserManager
        private static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> users) where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            // Mock CreateAsync method with callback to add user to the list
            mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<TUser, string>((x, y) => users.Add(x));

            mgr.Setup(x => x.AddToRoleAsync(It.IsAny<TUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            return mgr;
        }

        [Fact]
        public async Task SendInvitationAsync_ShouldSendEmail_WhenValidInvitation()
        {
            // Arrange
            var email = "test@example.com";
            var fullName = "John Doe";

            // Set up mock repository behavior
            _mockInvitationRepo.Setup(repo => repo.SaveInvitationAsync(It.IsAny<Invitation>())).Returns(Task.CompletedTask);
            _mockEmailService.Setup(service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            var returnedId = await _userService.SendInvitationAsync(email, fullName);

            // Assert: Verify that the invitation was saved and email was sent
            _mockInvitationRepo.Verify(repo => repo.SaveInvitationAsync(It.IsAny<Invitation>()), Times.Once);
            _mockEmailService.Verify(service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            
        }

        [Fact]
        public async Task CompleteRegistrationAsync_ShouldThrowException_WhenInvalidToken()
        {
            // Arrange
            var invalidToken = "invalidToken";
            var password = "password123";

            // Mock the invitation repository to return null for an invalid token
            _mockInvitationRepo.Setup(repo => repo.GetInvitationByTokenAsync(invalidToken)).ReturnsAsync((Invitation)null);

            // Act & Assert: Expect exception when the token is invalid
            await Assert.ThrowsAsync<Exception>(() => _userService.CompleteRegistrationAsync(invalidToken, password));
        }

        [Fact]
        public async Task CompleteRegistrationAsync_ShouldThrowException_WhenExpiredToken()
        {
            // Arrange
            var expiredToken = "expiredToken";
            var password = "password123";
            var invitation = new Invitation
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                Token = expiredToken,
                FullName = "John Doe",
                ExpirationDate = DateTime.UtcNow.AddDays(-1) // Token is expired
            };

            _mockInvitationRepo.Setup(repo => repo.GetInvitationByTokenAsync(expiredToken)).ReturnsAsync(invitation);

            // Act & Assert: Expect exception when the token is expired
            await Assert.ThrowsAsync<Exception>(() => _userService.CompleteRegistrationAsync(expiredToken, password));
        }

        [Fact]
        public async Task CompleteRegistrationAsync_ShouldCreateUser_WhenValidToken()
        {
            // Arrange
            var token = "validToken";
            var password = "password123";
            var invitation = new Invitation
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                Token = token,
                FullName = "John Doe",
                ExpirationDate = DateTime.UtcNow.AddDays(1) // Token is valid
            };

            // Set up mock repository behavior
            _mockInvitationRepo.Setup(repo => repo.GetInvitationByTokenAsync(token)).ReturnsAsync(invitation);

            // Act
            await _userService.CompleteRegistrationAsync(token, password);

            // Assert: Verify user creation and role assignment
            Assert.Equal(3, _users.Count); // 2 existing users + 1 new user
            Assert.Contains(_users, user => user.Email == invitation.Email);
            _mockUserManager.Verify(manager => manager.AddToRoleAsync(It.Is<User>(u => u.UserName == invitation.Email), "Student"), Times.Once);
            _mockInvitationRepo.Verify(repo => repo.DeleteInvitationAsync(token), Times.Once);
        }

        [Fact]
        public async Task GetValidInvitationAsync_ShouldReturnInvitation_WhenValidToken()
        {
            // Arrange
            var token = "validToken";
            var invitation = new Invitation
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                Token = token,
                FullName = "John Doe",
                ExpirationDate = DateTime.UtcNow.AddDays(1) // Token is valid
            };

            _mockInvitationRepo.Setup(repo => repo.GetInvitationByTokenAsync(token)).ReturnsAsync(invitation);

            // Act
            var result = await _userService.GetValidInvitationAsync(token);

            // Assert: Verify that the invitation is returned
            Assert.NotNull(result);
            Assert.Equal(token, result.Token);
        }

        [Fact]
        public async Task GetValidInvitationAsync_ShouldReturnNull_WhenInvalidToken()
        {
            // Arrange
            var token = "invalidToken";
            _mockInvitationRepo.Setup(repo => repo.GetInvitationByTokenAsync(token)).ReturnsAsync((Invitation)null);

            // Act
            var result = await _userService.GetValidInvitationAsync(token);

            // Assert: Verify that the result is null
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnAllUsers()
        {
            // Arrange
            var userDtos = new List<UserDTO>
            {
                new UserDTO { Id = Guid.NewGuid().ToString(), FullName = "User One", DateOfBirth = DateTime.UtcNow.AddYears(-25) },
                new UserDTO { Id = Guid.NewGuid().ToString(), FullName = "User Two", DateOfBirth = DateTime.UtcNow.AddYears(-30) }
            };

            _mockUserRepo.Setup(repo => repo.GetAllUsersAsync()).ReturnsAsync(_users);

            // Act
            var result = await _userService.GetAllUsers();

            // Assert: Verify the list of users is returned correctly
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetUserById_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var userId = _users[0].Id;
            _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(Guid.Parse(userId))).ReturnsAsync(_users[0]);

            // Act
            var result = await _userService.GetUserById(Guid.Parse(userId));

            // Assert: Verify the correct user is returned
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid(); // Non-existent user ID
            _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetUserById(userId);

            // Assert: Verify that no user is returned
            Assert.Null(result);
        }
    }
}
