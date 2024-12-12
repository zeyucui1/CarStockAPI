using CarStockAPI.Models;
using CarStockAPI.Helpers;
using Microsoft.Data.Sqlite;
using Dapper;
using FastEndpoints;

namespace CarStockAPI.Controllers;

public class AuthController : Endpoint<Dealer>
{
    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Dealer req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
        {
            
            await SendAsync(new
            {
                Error = "Username and password cannot be empty."
            }, statusCode: 400); // Bad Request
            return;
        }

        try
        {
            using var connection = new SqliteConnection("Data Source=CarStock.db");
           var query = "SELECT * FROM Dealers WHERE Username = @Username AND Password = @Password LIMIT 1";
           var dealer = await connection.QuerySingleOrDefaultAsync<Dealer>(query, new { req.Username, req.Password });

            if (dealer == null)
            {
                // Unauthorized
                await SendAsync(new
                {
                    Error = "Invalid username or password."
                }, statusCode: 401); // Unauthorized
                return;
            }

            // generate token
            var token = JwtHelper.GenerateToken(dealer.Id, dealer.Username);

            // return token
            await SendAsync(new { Token = token });
        }
        catch (Exception ex)
        {
            //  Internal Server Error
            await SendAsync(new
            {
                Error = $"An error occurred: {ex.Message}"
            }, statusCode: 500); // Internal Server Error
        }
    }
}