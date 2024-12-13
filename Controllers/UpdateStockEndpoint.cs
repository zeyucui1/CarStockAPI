using System.Security.Claims;
using Microsoft.Data.Sqlite;
using Dapper;
using FastEndpoints;

namespace CarStockAPI.Endpoints.Cars;

public class UpdateCarStockEndpoint : Endpoint<UpdateCarStockRequest>
{
    public override void Configure()
    {
        Put("/cars/update-stock"); 
        Roles("Dealer");           
    }

    public override async Task HandleAsync(UpdateCarStockRequest req, CancellationToken ct)
    {
        // input data validation
        if (req.Id <= 0 || req.Stock < 0)
        {
            AddError("Invalid input data. Vehicle ID and stock must be valid.");
            await SendErrorsAsync(400); 
            return;
        }

        try
        {
            // get the DealerId from the authentication token
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

            using var connection = new SqliteConnection("Data Source=CarStock.db");

            // check if the car exists and belongs to the dealer
            var checkQuery = "SELECT COUNT(*) FROM Cars WHERE Id = @Id AND DealerId = @DealerId";
            var count = await connection.ExecuteScalarAsync<int>(checkQuery, new { req.Id, DealerId = dealerId });

            if (count == 0)
            {
                AddError("Unauthorized or car not found.");
                await SendErrorsAsync(404); // Not Found
                return;
            }

            // update the stock
            var updateQuery = "UPDATE Cars SET Stock = @Stock WHERE Id = @Id AND DealerId = @DealerId";
            await connection.ExecuteAsync(updateQuery, new { req.Id, DealerId = dealerId, req.Stock });

            // success response
            await SendAsync(new { message = "Stock updated successfully." }, 200);
        }
        catch (Exception ex)
        {
            AddError($"An error occurred: {ex.Message}");
            await SendErrorsAsync(500); // Internal Server Error
        }
    }
}

// 请求数据的模型
public class UpdateCarStockRequest
{
    public int Id { get; set; }    
    public int Stock { get; set; } 
}