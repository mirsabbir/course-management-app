using CourseManagement.Application.DTOs;
using CourseManagement.Application.Interfaces;
using CourseManagement.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CourseManagement.UnitTests.InfrastructureTests
{
    public class UserServiceTests
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _mockHttpClient;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _mockHttpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _mockConfiguration = new Mock<IConfiguration>();

            _userService = new UserService(_mockHttpClient, _mockConfiguration.Object);
        }

        private void SetupGetAccessTokenAsync(string token = "mock-access-token")
        {
            var authUrl = "https://auth-server.com/token";
            var clientId = "client-id";
            var clientSecret = "client-secret";
            var tokenResponse = new { access_token = token };

            _mockConfiguration
                .Setup(config => config["AuthServer:Url"])
                .Returns(authUrl);

            _mockConfiguration
                .Setup(config => config["AuthServer:ClientId"])
                .Returns(clientId);

            _mockConfiguration
                .Setup(config => config["AuthServer:ClientSecret"])
                .Returns(clientSecret);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString() == authUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(tokenResponse))
                });
        }

        [Fact]
        public async Task CreateUserAsync_ShouldReturnUserId()
        {
            // Arrange
            var userDTO = new CreateUserDTO { FullName = "Test User", Email = "test@example.com" };
            var createUserUrl = "https://user-service.com/create";
            var token = "mock-access-token";
            var createdUserResponse = new { userId = "12345" };

            // Mock GetAccessTokenAsync
            SetupGetAccessTokenAsync(token);

            _mockConfiguration
                .Setup(config => config["UserService:CreateUserUrl"])
                .Returns(createUserUrl);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString() == createUserUrl &&
                        req.Headers.Authorization.Scheme == "Bearer" &&
                        req.Headers.Authorization.Parameter == token),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(createdUserResponse))
                });

            // Act
            var userId = await _userService.CreateUserAsync(userDTO);

            // Assert
            Assert.Equal("12345", userId);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnListOfUsers()
        {
            // Arrange
            var getAllUsersUrl = "https://user-service.com/users";
            var token = "mock-access-token";
            var users = new List<UserDTO>
            {
                new UserDTO { Id = Guid.NewGuid(), FullName = "User 1" },
                new UserDTO { Id = Guid.NewGuid(), FullName = "User 2" }
            };

            // Mock GetAccessTokenAsync
            SetupGetAccessTokenAsync(token);

            _mockConfiguration
                .Setup(config => config["UserService:GetAllUsersUrl"])
                .Returns(getAllUsersUrl);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString() == getAllUsersUrl &&
                        req.Headers.Authorization.Scheme == "Bearer" &&
                        req.Headers.Authorization.Parameter == token),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(users))
                });

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var getUserByIdUrl = $"https://user-service.com/users/{userId}";
            var token = "mock-access-token";
            var user = new UserDTO { Id = userId, FullName = "Test User" };

            // Mock GetAccessTokenAsync
            SetupGetAccessTokenAsync(token);

            _mockConfiguration
                .Setup(config => config["UserService:GetUserByIdUrl"])
                .Returns("https://user-service.com/users/{0}");

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString() == getUserByIdUrl &&
                        req.Headers.Authorization.Scheme == "Bearer" &&
                        req.Headers.Authorization.Parameter == token),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(user))
                });

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("Test User", result.FullName);
        }
    }
}