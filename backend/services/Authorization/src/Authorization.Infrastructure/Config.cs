using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using System.Security.Claims;

namespace Authorization.Infrastructure;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
    ];

    public static IEnumerable<ApiScope> ApiScopes =>
    [
        new ApiScope("user.manage", "manage users"),
        new ApiScope("course.manage", "manage courses"),
    ];

    public static IEnumerable<Client> Clients =>
    [
        new Client
        {
            ClientId = "course-management-api",
            Description = "Client for M2M authentication for dotnet course management webapi",
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets =
            {
                new Secret("secret".Sha256())
            },
            AllowedScopes = { "user.manage" }
        },
        new Client
        {
            ClientId = "frontend-app",
            ClientName = "Course Management React App",
            AllowedGrantTypes = GrantTypes.Code,
            RequirePkce = true,
            RequireClientSecret = false,
            ClientSecrets =
            {
                new Secret("secret".Sha256())
            },
            RedirectUris = { "http://localhost:3000/callback" },
            PostLogoutRedirectUris = { "http://localhost:3000/signout-callback-oidc" },
            AllowedScopes = { "openid", "profile", "course.manage" },
            AllowOfflineAccess = true
        },
        new Client
        {
            ClientId = "integration-test",
            Description = "Client for M2M authentication for integration test",
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets =
            {
                new Secret("secret".Sha256())
            },
            AllowedScopes = { "course.manage" }
        },
    ];

    public static List<TestUser> Users => new List<TestUser>
    {
        new TestUser
        {
            SubjectId = "1",
            Username = "alice",
            Password = "alice",
            Claims = new List<Claim>
            {
                new Claim("name", "Alice"),
                new Claim("website", "https://alice.com")
            }
        },
        new TestUser
        {
            SubjectId = "2",
            Username = "bob",
            Password = "bob",
            Claims = new List<Claim>
            {
                new Claim("name", "Bob"),
                new Claim("website", "https://bob.com")
            }
        }
    };
}