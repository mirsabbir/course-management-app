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
        private readonly string _token;

        public ClassControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _token = GetJwtTokenAsync().GetAwaiter().GetResult();
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string url, object content = null)
        {
            var request = new HttpRequestMessage(method, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            if (content != null)
            {
                request.Content = JsonContent.Create(content);
            }

            return request;
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            var response = await _client.SendAsync(CreateRequest(HttpMethod.Get, "/api/classes"));
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenClassDoesNotExist()
        {
            var response = await _client.SendAsync(CreateRequest(HttpMethod.Get, $"/api/classes/{Guid.NewGuid()}"));
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create_ShouldReturnCreated_WhenValidRequest()
        {
            var request = new CreateClassDTO
            {
                Name = "Mathematics 101",
                Description = "Basic math concepts",
            };

            var response = await _client.SendAsync(CreateRequest(HttpMethod.Post, "/api/classes", request));
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenInvalidRequest()
        {
            var classId = Guid.NewGuid();
            var request = new UpdateClassDTO
            {
                Name = "Updated Class Name",
                Description = "Updated Description",
            };

            var response = await _client.SendAsync(CreateRequest(HttpMethod.Put, $"/api/classes/{classId}", request));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Delete_ShouldReturnOk_WhenClassExists()
        {
            var classId = Guid.NewGuid();
            var response = await _client.SendAsync(CreateRequest(HttpMethod.Delete, $"/api/classes/{classId}"));
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetCoursesByClassId_ShouldReturnNotFound_WhenClassDoesNotExist()
        {
            var response = await _client.SendAsync(CreateRequest(HttpMethod.Get, $"/api/classes/{Guid.NewGuid()}/courses"));
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetStudentsByClassId_ShouldReturnNotFound_WhenClassDoesNotExist()
        {
            var response = await _client.SendAsync(CreateRequest(HttpMethod.Get, $"/api/classes/{Guid.NewGuid()}/students"));
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
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
