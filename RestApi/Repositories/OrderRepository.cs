using System.Data.SqlClient;
using RestApi.Models;

namespace RestApi.Repositories;

public interface IOrderRepository
{
    public Task<Order?> GetOrderByProductIdAndAmount(int idProduct, int amount);
}

public class OrderRepository(IConfiguration configuration) : IOrderRepository
{
    public async Task<Order?> GetOrderByProductIdAndAmount(int idProduct, int amount)
    {
        await using var connection = new SqlConnection(configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();

        await using var command = new SqlCommand(
            @"SELECT * FROM ""Order"" WHERE IdProduct = @IdProduct AND Amount = @amount",
            connection);
        command.Parameters.AddWithValue("@IdProduct", idProduct);
        command.Parameters.AddWithValue("@amount", amount);
        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync() != true) return null;
        var order = new Order
        {
            IdOrder = (int)reader["IdOrder"],
            IdProduct = (int)reader["IdProduct"],
            Amount = (int)reader["Amount"],
            CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString()!),
            FulfilledAt = reader["FulfilledAt"] == DBNull.Value
                ? null
                : DateTime.Parse(reader["FulfilledAt"].ToString()!)
        };
        return order;
    }
}