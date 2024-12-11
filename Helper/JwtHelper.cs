using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CarStockAPI.Helpers;

public static class JwtHelper
{
    
    private static readonly string SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "your secret key";

    public static string GenerateToken(int dealerId, string username)
    {
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, dealerId.ToString()),
            new Claim(ClaimTypes.Name, username)
        };

       
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

       
        var token = new JwtSecurityToken(
            issuer: "CarStockAPI",
            audience: "CarStockAPI",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds);

       
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static ClaimsPrincipal ValidateToken(string token)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var tokenHandler = new JwtSecurityTokenHandler();

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, 
            ValidateAudience = true, 
            ValidIssuer = "CarStockAPI",
            ValidAudience = "CarStockAPI",
            ValidateLifetime = true, 
            IssuerSigningKey = key
        };

        
        return tokenHandler.ValidateToken(token, validationParameters, out _);
    }
}