using System.Security.Claims;
using Microsoft.Data.Sqlite;
using Dapper;
using FastEndpoints;

namespace CarStockAPI.Endpoints.Cars;

public class RemoveCarEndpoint : Endpoint<RemoveCarRequest>
{
    public override void Configure()
    {
        Delete("/cars/{id}"); // route parameter
        Roles("Dealer"); 
        
    }

    public override async Task HandleAsync(RemoveCarRequest req, CancellationToken ct)
    {
        // check if the input data is valid
        if (req.Id <= 0)
        {
            AddError("Invalid car ID.");
            await SendErrorsAsync(400); 
            return;
        }

        try
        {
            // get the DealerId from the authentication token
            var dealerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

           
            if (string.IsNullOrEmpty(dealerIdClaim))
            {
                AddError("No valid authentication token found.");
                await SendErrorsAsync(401);
                return;
            }

            if (!int.TryParse(dealerIdClaim, out int dealerId))
            {
                AddError("Invalid Dealer ID in authentication token.");
                await SendErrorsAsync(401);
                return;
            }

            using var connection = new SqliteConnection("Data Source=CarStock.db");
            
            // check if the car exists and belongs to the dealer
            var queryCheck = "SELECT COUNT(*) FROM Cars WHERE Id = @Id AND DealerId = @DealerId";
            var count = await connection.ExecuteScalarAsync<int>(queryCheck, new { req.Id, DealerId = dealerId });
            
            if (count == 0)
            {
                AddError("Unauthorized or car not found.");
                await SendErrorsAsync(404);
                return;
            }

            //delete the car
            var queryDelete = "DELETE FROM Cars WHERE Id = @Id AND DealerId = @DealerId";
            await connection.ExecuteAsync(queryDelete, new { req.Id, DealerId = dealerId });

         
            await SendAsync(new { message = "Car deleted successfully." }, 200);
        }
        catch (Exception ex)
        {
            AddError($"An error occurred: {ex.Message}");
            await SendErrorsAsync(500);
        }
    }
}

// 请求数据的模型
public class RemoveCarRequest
{
    public int Id { get; set; } // 车辆 ID
}