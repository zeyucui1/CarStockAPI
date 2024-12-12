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
                // Validate the JWT and retrieve claims
                var principal = JwtHelper.ValidateToken(token);
                context.User = principal;

                // Extract DealerId from the JWT claims
                var dealerIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(dealerIdClaim) && int.TryParse(dealerIdClaim, out var dealerId))
                {
                    // Store DealerId in HttpContext for later use
                    context.Items["DealerId"] = dealerId;
                }
                else
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    await context.Response.WriteAsync("Invalid Token: Missing DealerId");
                    return;
                }
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