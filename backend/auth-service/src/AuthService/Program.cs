using AuthService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Register the ApplicationDbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(""));

// Add ASP.NET Core Identity with Entity Framework stores
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


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
.AddTestUsers(Config.Users)
.AddDeveloperSigningCredential();

builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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