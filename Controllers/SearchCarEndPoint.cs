using System.Security.Claims;
using Microsoft.Data.Sqlite;
using Dapper;
using FastEndpoints;

namespace CarStockAPI.Endpoints.Cars.Search;

public class SearchCarEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/cars/search"); 
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

            // get the search parameters from the query string
            HttpContext.Request.Query.TryGetValue("make", out var makeQuery);
            HttpContext.Request.Query.TryGetValue("model", out var modelQuery);

            var make = makeQuery.ToString();
            var model = modelQuery.ToString();

            // check if at least one parameter is provided
            if (string.IsNullOrWhiteSpace(make) && string.IsNullOrWhiteSpace(model))
            {
                AddError("At least one parameter (make or model) must be provided.");
                await SendErrorsAsync(400); // Bad Request
                return;
            }

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
                Make = string.IsNullOrWhiteSpace(make) ? null : make,
                Model = string.IsNullOrWhiteSpace(model) ? null : model
            });

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