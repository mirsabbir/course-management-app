using CourseManagement.Application.Interfaces;
using CourseManagement.Application.Services;
using CourseManagement.Infrastructure;
using CourseManagement.Infrastructure.Repositories;
using CourseManagement.Infrastructure.Services;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using CourseManagement.Application.DTOs.Classes;
using CourseManagement.Application.Validators;
using CourseManagement.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// add repositories
builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();

// add services
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IUserService, UserService>();


if (builder.Environment.IsEnvironment("IntegrationTest"))
{
    // Use in-memory database for testing
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseInMemoryDatabase("InMemoryDbForTesting");
    });
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
       options.UseNpgsql(
               builder.Configuration["DatabaseConnectionString"],
               npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "CourseManagement"))
              .EnableSensitiveDataLogging() // Only for debugging
              .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
}

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient();

// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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

// add fluent validation
builder.Services.AddValidatorsFromAssemblyContaining<CreateClassDtoValidator>();
builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();

// Add Serilog logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "openapi/v1.json";
    });
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


public partial class Program { }
