using System.Data.SqlClient;
using RestApi.Models;

namespace RestApi.Repositories;

public interface IDeliveryRepository
{
    public Task<Delivery?> AddDelivery(
        int idWarehouse,
        int idProduct,
        int idOrder,
        int amount,
        decimal price,
        DateTime createdAt
    );

    public Task<Delivery?> GetDeliveryByOrderId(int idOrder);
}

public class DeliveryRepository(IConfiguration configuration) : IDeliveryRepository
{
    public async Task<Delivery?> AddDelivery(
        int idWarehouse,
        int idProduct,
        int idOrder,
        int amount,
        decimal price,
        DateTime createdAt
    )
    {
        await using var connection = new SqlConnection(configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var updateQuery = @"UPDATE ""Order"" SET FulfilledAt = @FulfilledAt WHERE IdOrder = @IdOrder;";
            await using var updateCommand = new SqlCommand(updateQuery, connection);
            updateCommand.Transaction = (SqlTransaction)transaction;
            updateCommand.Parameters.AddWithValue("@IdOrder", idOrder);
            updateCommand.Parameters.AddWithValue("@FulfilledAt", DateTime.UtcNow);
            var updatedRows = await updateCommand.ExecuteNonQueryAsync();

            if (updatedRows != 1)
            {
                await transaction.RollbackAsync();
                return null;
            }

            var insertQuery =
                @"INSERT INTO ""Product_Warehouse"" (IdWarehouse, IdProduct, IdOrder, CreatedAt, Amount, Price)
                                OUTPUT INSERTED.*
                                VALUES (@IdWarehouse, @IdProduct, @IdOrder, @CreatedAt, @Amount, @Price);";
            await using var insertCommand = new SqlCommand(insertQuery, connection);
            insertCommand.Transaction = (SqlTransaction)transaction;
            insertCommand.Parameters.AddWithValue("@IdWarehouse", idWarehouse);
            insertCommand.Parameters.AddWithValue("@IdProduct", idProduct);
            insertCommand.Parameters.AddWithValue("@IdOrder", idOrder);
            insertCommand.Parameters.AddWithValue("@Amount", amount);
            insertCommand.Parameters.AddWithValue("@Price", price);
            insertCommand.Parameters.AddWithValue("@CreatedAt", createdAt);
            await using var reader = await insertCommand.ExecuteReaderAsync();

            if (await reader.ReadAsync() != true)
            {
                await transaction.RollbackAsync();
                return null;
            }

            var delivery = new Delivery
            {
                IdProductWarehouse = (int)reader["IdProductWarehouse"],
                IdWarehouse = (int)reader["IdWarehouse"],
                IdProduct = (int)reader["IdProduct"],
                IdOrder = (int)reader["IdOrder"],
                Amount = (int)reader["Amount"],
                Price = (decimal)reader["Price"],
                CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString()!)
            };

            await reader.CloseAsync();
            await transaction.CommitAsync();
            return delivery;
        }
        catch
        {
            await transaction.RollbackAsync();
            return null;
        }
    }

    public async Task<Delivery?> GetDeliveryByOrderId(int idOrder)
    {
        await using var connection = new SqlConnection(configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();

        await using var command = new SqlCommand("SELECT * FROM Product_Warehouse WHERE IdOrder = @IdOrder", connection);
        command.Parameters.AddWithValue("@IdOrder", idOrder);
        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync() != true) return null;
        var delivery = new Delivery
        {
            IdProductWarehouse = (int)reader["IdProductWarehouse"],
            IdWarehouse = (int)reader["IdWarehouse"],
            IdProduct = (int)reader["IdProduct"],
            IdOrder = (int)reader["IdOrder"],
            Amount = (int)reader["Amount"],
            Price = (decimal)reader["Price"],
            CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString()!)
        };
        return delivery;
    }
}