using System.Data.SqlClient;
using RestApi.Models;

namespace RestApi.Repositories;

public interface IDeliveryRepository
{
    public Delivery? AddDelivery(int idWarehouse, int idProduct, int idOrder, int amount, decimal price,
        DateTime createdAt);
}

public class DeliveryRepository(IConfiguration configuration) : IDeliveryRepository
{
    public Delivery? AddDelivery(int idWarehouse, int idProduct, int idOrder, int amount, decimal price,
        DateTime createdAt)
    {
        using var connection = new SqlConnection(configuration["ConnectionStrings:DefaultConnection"]);
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            var updateQuery = @"UPDATE ""Order"" SET FulfilledAt = @FulfilledAt WHERE IdOrder = @IdOrder;";
            using var updateCommand = new SqlCommand(updateQuery, connection);
            updateCommand.Transaction = transaction;
            updateCommand.Parameters.AddWithValue("@IdOrder", amount);
            updateCommand.Parameters.AddWithValue("@FulfilledAt", DateTime.UtcNow);
            updateCommand.ExecuteNonQuery();

            var insertQuery = @"INSERT INTO ""Product_Warehouse"" (IdWarehouse, IdProduct, IdOrder, CreatedAt, Amount, Price)
                                OUTPUT INSERTED.*
                                VALUES (@IdWarehouse, @IdProduct, @IdOrder, @CreatedAt, @Amount, @Price);";
            using var insertCommand = new SqlCommand(insertQuery, connection);
            insertCommand.Transaction = transaction;
            insertCommand.Parameters.AddWithValue("@IdWarehouse", idWarehouse);
            insertCommand.Parameters.AddWithValue("@IdProduct", idProduct);
            insertCommand.Parameters.AddWithValue("@IdOrder", idOrder);
            insertCommand.Parameters.AddWithValue("@Amount", amount);
            insertCommand.Parameters.AddWithValue("@Price", price);
            insertCommand.Parameters.AddWithValue("@CreatedAt", createdAt);
            var reader = insertCommand.ExecuteReader();

            if (!reader.Read())
            {
                transaction.Rollback();
                return null;
            }

            var delivery = new Delivery()
            {
                IdWarehouse = (int)reader["IdWarehouse"],
                IdProduct = (int)reader["IdProduct"],
                IdOrder = (int)reader["IdOrder"],
                Amount = (int)reader["Amount"],
                Price = (decimal)reader["Price"],
                CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString()!)
            };

            reader.Close();
            transaction.Commit();
            return delivery;
        }
        catch
        {
            transaction.Rollback();
            return null;
        }
    }
}