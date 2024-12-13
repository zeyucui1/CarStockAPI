using System.Security.Claims;
using Microsoft.Data.Sqlite;
using Dapper;
using FastEndpoints;

namespace CarStockAPI.Endpoints.Cars;

public class ListCarsEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/cars/list"); 
        Roles("Dealer");  
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        try
        {
           
            var dealerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            
            if (string.IsNullOrEmpty(dealerIdClaim))
            {
                AddError("Unauthorized access.");
                await SendErrorsAsync(401);
                return;
            }

            if (!int.TryParse(dealerIdClaim, out int dealerId))
            {
                AddError("Unauthorized access.");
                await SendErrorsAsync(401);
                return;
            }

            // search parameters
            var make = HttpContext.Request.Query.TryGetValue("make", out var makeQuery) ? makeQuery.ToString() : null;
            var model = HttpContext.Request.Query.TryGetValue("model", out var modelQuery) ? modelQuery.ToString() : null; 

            using var connection = new SqliteConnection("Data Source=CarStock.db");

            // use dapper to query the database
            var query = @"
                SELECT Id, Make, Model, Year, Stock
                FROM Cars
                WHERE DealerId = @DealerId
                AND (@Make IS NULL OR Make = @Make)
                AND (@Model IS NULL OR Model = @Model)";
            
            var cars = await connection.QueryAsync<CarResponse>(query, new
            {
                DealerId = dealerId,
                Make = string.IsNullOrEmpty(make) ? null : make,
                Model = string.IsNullOrEmpty(model) ? null : model
            });

            // success response
            await SendAsync(cars, 200);
        }
        catch (Exception ex)
        {
            AddError($"An error occurred: {ex.Message}");
            await SendErrorsAsync(500);
        }
    }
}


public class CarResponse
{
    public int Id { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public int Stock { get; set; }
}