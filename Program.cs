using CarStockAPI.Helpers;
using CarStockAPI.Middleware;
using FastEndpoints; 
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);
// Load environment variables
DotNetEnv.Env.Load();

builder.Services.AddFastEndpoints();

// initialize database
DatabaseInitializer.InitializeDatabase();

var app = builder.Build();

// JWT Middleware
app.UseMiddleware<AuthMiddleware>();

// use FastEndpoints
app.UseFastEndpoints();

app.Run();