using Authorization.Application.Interfaces;
using Authorization.Application.Services;
using Authorization.Domain;
using Authorization.Infrastructure;
using Authorization.Infrastructure.Repositories;
using Authorization.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Register Application Layer services
builder.Services.AddScoped<IUserService, UserService>();

// Register Infrastructure Layer services
builder.Services.AddScoped<IInvitationRepository, InvitationRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Register the ApplicationDbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration["DatabaseConnectionString"]));

// Add ASP.NET Core Identity with Entity Framework stores
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Duende Identity Server configuration
builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;
    options.EmitStaticAudienceClaim = true;
})
.AddInMemoryIdentityResources(Config.IdentityResources)
.AddInMemoryApiScopes(Config.ApiScopes)
.AddInMemoryClients(Config.Clients)
.AddAspNetIdentity<User>()
.AddProfileService<ProfileService>()
.AddDeveloperSigningCredential();

// login/invitation/concent page UI
builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // IdentityServer authority (the base URL of your IdentityServer [iss])
    options.Authority = "http://localhost:5161";
    options.RequireHttpsMetadata = false;

    // Audience of the token (must match the API resource name in IdentityServer [aud])
    options.Audience = "http://localhost:5161/resources";

    // Token validation parameters
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero
    };

    // Enable token validation events
    options.Events = new JwtBearerEvents
    {
        // only for debugging
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Authentication failed: " + context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated: " + context.SecurityToken);
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// Serve static files (CSS, JS, images, etc.)
app.UseStaticFiles();

// Enable routing
app.UseRouting();

// Add IdentityServer middleware (to enable the endpoints e.g. /connect/token)
app.UseIdentityServer();

// Enable authentication (ASP .NET Identity) 
app.UseAuthentication();

// Enable authorization
app.UseAuthorization();

// Map Razor Pages (for Quickstart UI)
app.MapRazorPages();

// Map controllers (for API endpoints)
app.MapControllers();

app.Run();