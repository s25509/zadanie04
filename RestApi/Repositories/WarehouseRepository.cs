using System.Data.SqlClient;
using RestApi.Models;

namespace RestApi.Repositories;

public interface IWarehouseRepository
{
    public Warehouse? GetWarehouseById(int idWarehouse);
}

public class WarehouseRepository(IConfiguration configuration) : IWarehouseRepository
{
    public Warehouse? GetWarehouseById(int idWarehouse)
    {
        using var connection = new SqlConnection(configuration["ConnectionStrings:DefaultConnection"]);
        connection.Open();

        var command = new SqlCommand("SELECT * FROM Warehouse WHERE IdWarehouse = @IdWarehouse", connection);
        command.Parameters.AddWithValue("@IdWarehouse", idWarehouse);
        using var reader = command.ExecuteReader();

        if (!reader.Read()) return null;
        var warehouse = new Warehouse
        {
            IdWarehouse = (int)reader["IdWarehouse"],
            Name = reader["Name"].ToString()!,
            Address = reader["Address"].ToString()!
        };
        return warehouse;
    }
}