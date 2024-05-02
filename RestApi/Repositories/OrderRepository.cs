using System.Data.SqlClient;
using RestApi.Models;

namespace RestApi.Repositories;

public interface IOrderRepository
{
    public Order? GetOrderByProductIdAndAmount(int idProduct, int amount);
}

public class OrderRepository(IConfiguration configuration) : IOrderRepository
{
    public Order? GetOrderByProductIdAndAmount(int idProduct, int amount)
    {
        using var connection = new SqlConnection(configuration["ConnectionStrings:DefaultConnection"]);
        connection.Open();

        var command = new SqlCommand(@"SELECT * FROM ""Order"" WHERE IdProduct = @IdProduct AND Amount = @amount",
            connection);
        command.Parameters.AddWithValue("@IdProduct", idProduct);
        command.Parameters.AddWithValue("@amount", amount);
        using var reader = command.ExecuteReader();

        if (!reader.Read()) return null;
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