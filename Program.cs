using CarStockAPI.Helpers;
using CarStockAPI.Middleware;
using FastEndpoints; 
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables
DotNetEnv.Env.Load();

// Add authentication services for JWT
// Add authentication services for JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "CarStockAPI",
            ValidAudience = "CarStockAPI",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET") ?? "YourSuperSecureSecretKey123!"))
        };
    });

// Add authorization services
builder.Services.AddAuthorization();

// Add FastEndpoints services
builder.Services.AddFastEndpoints();

// Initialize database
DatabaseInitializer.InitializeDatabase();

var app = builder.Build();

// Enable authentication middleware
app.UseAuthentication();

// Enable authorization middleware
app.UseAuthorization();

// Use custom AuthMiddleware
app.UseMiddleware<AuthMiddleware>();

// Use FastEndpoints
app.UseFastEndpoints();

app.Run();