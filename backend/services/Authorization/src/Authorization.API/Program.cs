using Authorization.API.Middlewares;
using Authorization.API.Pages;
using Authorization.Application.Interfaces;
using Authorization.Application.Services;
using Authorization.Application.Validators;
using Authorization.Domain;
using Authorization.Infrastructure;
using Authorization.Infrastructure.Repositories;
using Authorization.Infrastructure.Services;
using Duende.IdentityServer;
using FluentValidation;
using FluentValidation.AspNetCore;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Log = Serilog.Log;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Register Application Layer services
builder.Services.AddScoped<IUserService, UserService>();

// Register Infrastructure Layer services
builder.Services.AddScoped<IInvitationRepository, InvitationRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISmtpClient, SmtpClient>();

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
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";

})
.AddJwtBearer(options =>
{
    // IdentityServer authority (the base URL of IdentityServer [iss])
    options.Authority = builder.Configuration["AuthServer:Url"];
    options.RequireHttpsMetadata = false;

    // Audience of the token (must match the API resource name in IdentityServer [aud])
    options.Audience = builder.Configuration["AuthServer:JwtAudience"];

    // Token validation parameters
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuers = [ "http://localhost:5161", "http://authorization-api:8080" ],
        ValidateAudience = true,
        ValidAudiences = ["http://localhost:5161/resources", "http://authorization-api:8080/resources"],
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

// Add Serilog logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // Allow all origins
              .AllowAnyHeader()  // Allow all headers
              .AllowAnyMethod(); // Allow all HTTP methods
    });
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensures the cookie is always secure
});

// add fluent validation
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserDtoValidator>();
builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();

var app = builder.Build();

app.UseCors("AllowAll"); // Apply the CORS policy


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "openapi/v1.json";
    });
}

// Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// Serve static files (CSS, JS, images, etc.)
app.UseStaticFiles();

app.UseMiddleware<ExceptionHandlingMiddleware>();

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