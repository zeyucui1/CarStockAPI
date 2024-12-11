using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using CarStockAPI.Helpers; 

namespace CarStockAPI.Middleware;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                //  use JwtHelper to validate token
                var principal = JwtHelper.ValidateToken(token);
                context.User = principal; 
            }
            catch
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("Invalid Token");
                return;
            }
        }

        
        await _next(context);
    }
}