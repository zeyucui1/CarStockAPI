using System.Security.Claims;
using Microsoft.Data.Sqlite;
using Dapper;
using FastEndpoints;

namespace CarStockAPI.Endpoints.Cars;

public class AddCarEndpoint : Endpoint<AddCarRequest>
{
    public override void Configure()
    {
        Post("/cars/add");
        Roles("Dealer"); 
        
    }

    public override async Task HandleAsync(AddCarRequest req, CancellationToken ct)
    {
        // check if the input data is valid
        if (string.IsNullOrWhiteSpace(req.Make) ||
            string.IsNullOrWhiteSpace(req.Model) ||
            req.Year < 1886 || req.Year > DateTime.Now.Year ||
            req.Stock < 0)
        {
            AddError("Invalid input data."); 
            await SendErrorsAsync(400); 
            return;
        }

        try
        {
            
            var dealerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (dealerIdClaim == null || !int.TryParse(dealerIdClaim, out int dealerId))
            {
                AddError("Unauthorized access."); 
                await SendErrorsAsync(401); 
                return;
            }

            // add the car to the database
            using var connection = new SqliteConnection("Data Source=CarStock.db");
            var query = @"
                INSERT INTO Cars (DealerId, Make, Model, Year, Stock)
                VALUES (@DealerId, @Make, @Model, @Year, @Stock)";

            await connection.ExecuteAsync(query, new
            {
                DealerId = dealerId,
                req.Make,
                req.Model,
                req.Year,
                req.Stock
            });

            await SendAsync(new { message = "Car added successfully." }, 201);
        }
        catch (Exception ex)
        {
            AddError($"An error occurred: {ex.Message}"); 
            await SendErrorsAsync(500); 
        }
    }
}


public class AddCarRequest
{
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public int Stock { get; set; }
}