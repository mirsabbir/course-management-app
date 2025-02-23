using CourseManagement.Application.DTOs.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;

namespace CourseManagement.IntegrationTests
{
    public class ClassControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ClassControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Create_ShouldReturnCreated_WhenValidRequest()
        {
            var token = await GetJwtTokenAsync();

            var request = new CreateClassDTO
            {
                Name = "Mathematics 101",
                Description = "Basic math concepts",
            };

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/classes")
            {
                Content = JsonContent.Create(request)  // Use JsonContent for creating the body from DTO
            };

            // Step 5: Set the Authorization header with the Bearer token
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Step 6: Send the request with the Authorization header
            var response = await _client.SendAsync(httpRequestMessage);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var createdClass = await response.Content.ReadFromJsonAsync<ClassDTO>();
            createdClass.Should().NotBeNull();
            createdClass.Name.Should().Be(request.Name);
        }

        public async Task<string> GetJwtTokenAsync()
        {
            var client = new HttpClient();

            // Define the token endpoint URL (assumed to be the same for all endpoints in your IdentityServer)
            var tokenEndpoint = "http://localhost:5161/connect/token";

            // Prepare the request body (client credentials flow)
            var requestData = new Dictionary<string, string>
            {
                { "client_id", "integration-test" },
                { "client_secret", "secret" },
                { "scope", "course.manage" }, 
                { "grant_type", "client_credentials" } 
            };

            var tokenRequest = new FormUrlEncodedContent(requestData);

            // Send the request to the token endpoint
            var response = await client.PostAsync(tokenEndpoint, tokenRequest);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error retrieving token: " + response.ReasonPhrase);
            }

            // Read the response and extract the token
            var tokenResponse = await response.Content.ReadAsStringAsync();

            // Deserialize the response (e.g., {"access_token":"YOUR_ACCESS_TOKEN", "expires_in":3600, ...})
            var tokenData = JsonConvert.DeserializeObject<Dictionary<string, string>>(tokenResponse);

            // Return the access token
            return tokenData["access_token"];
        }


    }

}
