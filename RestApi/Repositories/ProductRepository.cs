using System.Data.SqlClient;
using RestApi.Models;

namespace RestApi.Repositories;

public interface IProductRepository
{
    public Product? GetProductById(int idProduct);
}

public class ProductRepository(IConfiguration configuration) : IProductRepository
{
    public Product? GetProductById(int idProduct)
    {
        using var connection = new SqlConnection(configuration["ConnectionStrings:DefaultConnection"]);
        connection.Open();

        var command = new SqlCommand("SELECT * FROM Product WHERE IdProduct = @IdProduct", connection);
        command.Parameters.AddWithValue("@IdProduct", idProduct);
        using var reader = command.ExecuteReader();

        if (!reader.Read()) return null;
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