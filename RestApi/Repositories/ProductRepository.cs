using System.Data.SqlClient;
using RestApi.Models;

namespace RestApi.Repositories;

public interface IProductRepository
{
    public Task<Product?> GetProductById(int idProduct);
}

public class ProductRepository(IConfiguration configuration) : IProductRepository
{
    public async Task<Product?> GetProductById(int idProduct)
    {
        await using var connection = new SqlConnection(configuration["ConnectionStrings:DefaultConnection"]);
        await connection.OpenAsync();

        await using var command = new SqlCommand("SELECT * FROM Product WHERE IdProduct = @IdProduct", connection);
        command.Parameters.AddWithValue("@IdProduct", idProduct);
        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync() != true) return null;
        var product = new Product
        {
            IdProduct = (int)reader["IdProduct"],
            Name = reader["Name"].ToString()!,
            Description = reader["Description"].ToString()!,
            Price = (decimal)reader["Price"]
        };
        return product;
    }
}