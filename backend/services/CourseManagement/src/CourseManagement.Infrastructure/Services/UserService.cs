using CourseManagement.Application.DTOs;
using CourseManagement.Application.Interfaces;
using Microsoft.Extensions.Configuration;
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

        public UserService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var authUrl = _configuration["AuthServer:Url"];
            var clientId = _configuration["AuthServer:ClientId"];
            var clientSecret = _configuration["AuthServer:ClientSecret"];

            var requestBody = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", clientId },
                { "client_secret", clientSecret }
            };

            var response = await _httpClient.PostAsync(authUrl, new FormUrlEncodedContent(requestBody));
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

            return tokenResponse.access_token;
        }

        public async Task<string> CreateUserAsync(CreateUserDTO userDTO)
        {
            var token = await GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var userApiUrl = _configuration["UserService:CreateUserUrl"];
            var jsonContent = new StringContent(JsonConvert.SerializeObject(userDTO), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(userApiUrl, jsonContent);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var createdUserResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

            return createdUserResponse.userId;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var token = await GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var userApiUrl = _configuration["UserService:GetAllUsersUrl"];
            var response = await _httpClient.GetAsync(userApiUrl);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<UserDTO>>(responseContent);
        }

        public async Task<UserDTO> GetUserByIdAsync(Guid id)
        {
            var token = await GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var userApiUrl = string.Format(_configuration["UserService:GetUserByIdUrl"], id);
            var response = await _httpClient.GetAsync(userApiUrl);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserDTO>(responseContent);
        }
    }
}
