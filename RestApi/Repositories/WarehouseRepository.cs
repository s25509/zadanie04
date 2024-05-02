using System.Data.SqlClient;
using RestApi.Models;

namespace RestApi.Repositories;

public interface IWarehouseRepository
{
    public Task<Warehouse?> GetWarehouseById(int idWarehouse);
}

public class WarehouseRepository(IConfiguration configuration) : IWarehouseRepository
{
    public async Task<Warehouse?> GetWarehouseById(int idWarehouse)
    {
        await using var connection = new SqlConnection(configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();

        var command = new SqlCommand("SELECT * FROM Warehouse WHERE IdWarehouse = @IdWarehouse", connection);
        command.Parameters.AddWithValue("@IdWarehouse", idWarehouse);
        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync() != true) return null;
        var warehouse = new Warehouse
        {
            IdWarehouse = (int)reader["IdWarehouse"],
            Name = reader["Name"].ToString()!,
            Address = reader["Address"].ToString()!
        };
        return warehouse;
    }
}