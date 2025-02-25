using CourseManagement.Application.DTOs;
using CourseManagement.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;

        public UserService(HttpClient httpClient, IConfiguration configuration, ILogger<UserService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            _logger.LogInformation("Entering GetAccessTokenAsync method to fetch access token.");

            var authUrl = _configuration["AuthServer:TokenEndpoint"];
            var clientId = _configuration["AuthServer:ClientId"];
            var clientSecret = _configuration["AuthServer:ClientSecret"];

            var requestBody = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", clientId },
                { "client_secret", clientSecret }
            };

            try
            {
                _logger.LogInformation("Sending POST request to Auth Server URL: {AuthUrl} with client_id: {ClientId}", authUrl, clientId);

                var response = await _httpClient.PostAsync(authUrl, new FormUrlEncodedContent(requestBody));

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch access token. Status Code: {StatusCode}, Reason: {Reason}", response.StatusCode, response.ReasonPhrase);
                    response.EnsureSuccessStatusCode();
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

                _logger.LogInformation("Access token fetched successfully.");

                return tokenResponse.access_token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching access token.");
                throw; // Propagate the error
            }
        }

        public async Task<string> CreateUserAsync(CreateUserDTO userDTO)
        {
            _logger.LogInformation("Entering CreateUserAsync method to create a new user.");

            try
            {
                // Fetch the access token
                var token = await GetAccessTokenAsync();
                _logger.LogInformation("Fetched access token for user creation.");

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var userApiUrl = _configuration["UserService:CreateUserUrl"];
                var jsonContent = new StringContent(JsonConvert.SerializeObject(userDTO), Encoding.UTF8, "application/json");

                _logger.LogInformation("Sending POST request to create a user at URL: {UserApiUrl}. UserDTO: {UserDTO}", userApiUrl, userDTO);

                var response = await _httpClient.PostAsync(userApiUrl, jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to create user. Status Code: {StatusCode}, Reason: {Reason}", response.StatusCode, response.ReasonPhrase);
                    response.EnsureSuccessStatusCode();
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var createdUserResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
                string userId = createdUserResponse.userId;

                _logger.LogInformation("User created successfully with userId: {UserId}.", userId);

                return createdUserResponse.userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user.");
                throw; // Propagate the error
            }
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            _logger.LogInformation("Entering GetAllUsersAsync method to retrieve all users.");

            try
            {
                // Fetch the access token
                var token = await GetAccessTokenAsync();
                _logger.LogInformation("Fetched access token for retrieving users.");

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var userApiUrl = _configuration["UserService:GetAllUsersUrl"];
                _logger.LogInformation("Sending GET request to retrieve all users from URL: {UserApiUrl}", userApiUrl);

                var response = await _httpClient.GetAsync(userApiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to retrieve users. Status Code: {StatusCode}, Reason: {Reason}", response.StatusCode, response.ReasonPhrase);
                    response.EnsureSuccessStatusCode();
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<IEnumerable<UserDTO>>(responseContent);

                _logger.LogInformation("Successfully retrieved {UserCount} users.", users.Count());

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving users.");
                throw; // Propagate the error
            }
        }

        public async Task<UserDTO> GetUserByIdAsync(Guid id)
        {
            _logger.LogInformation("Entering GetUserByIdAsync method to retrieve user by ID: {UserId}", id);

            try
            {
                // Fetch the access token
                var token = await GetAccessTokenAsync();
                _logger.LogInformation("Fetched access token for retrieving user by ID: {UserId}", id);

                // Set the authorization header
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                // Format the URL to get the user by ID
                var userApiUrl = string.Format(_configuration["UserService:GetUserByIdUrl"], id);
                _logger.LogInformation("Sending GET request to URL: {UserApiUrl} to fetch user with ID: {UserId}", userApiUrl, id);

                // Send the HTTP request to get the user
                var response = await _httpClient.GetAsync(userApiUrl);

                // Log response status
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to retrieve user. Status Code: {StatusCode}, Reason: {Reason}", response.StatusCode, response.ReasonPhrase);
                    response.EnsureSuccessStatusCode();
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                // Deserialize the response into a UserDTO object
                var user = JsonConvert.DeserializeObject<UserDTO>(responseContent);

                _logger.LogInformation("Successfully retrieved user with ID: {UserId}.", id);

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving user by ID: {UserId}.", id);
                throw; // Propagate the error
            }
        }
    }
}
