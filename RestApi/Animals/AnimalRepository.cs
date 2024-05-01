using System.Data.SqlClient;
using System.Text;

namespace RestApi.Animals;

public interface IAnimalRepository
{
    public IEnumerable<Animal> FetchAllAnimals(string orderBy);

    public Animal? FetchAnimalById(int idAnimal);
    public Animal? CreateAnimal(string name, string? description, string category, string area);
    public Animal? UpdateAnimal(int idAnimal, string? name, string? description, string? category, string? area);
    public Animal? RemoveAnimal(int idAnimal);
}

public class AnimalRepository(IConfiguration configuration) : IAnimalRepository
{
    public static readonly string[] ValidOrderParameters = ["name", "description", "category", "area"];

    public IEnumerable<Animal> FetchAllAnimals(string orderBy)
    {
        using var connection = new SqlConnection(configuration["ConnectionStrings:DefaultConnection"]);
        connection.Open();

        var command = new SqlCommand($"SELECT * FROM Animal ORDER BY {orderBy}", connection);
        using var reader = command.ExecuteReader();

        var animals = new List<Animal>();
        while (reader.Read())
        {
            var animal = new Animal()
            {
                IdAnimal = (int)reader["IdAnimal"],
                Name = reader["Name"].ToString()!,
                Description = reader["Description"].ToString(),
                Category = reader["Category"].ToString()!,
                Area = reader["Area"].ToString()!,
            };
            animals.Add(animal);
        }

        return animals;
    }

    public Animal? FetchAnimalById(int idAnimal)
    {
        using var connection = new SqlConnection(configuration["ConnectionStrings:DefaultConnection"]);
        connection.Open();

        var command = new SqlCommand($"SELECT * FROM Animal WHERE idAnimal = @idAnimal", connection);
        command.Parameters.AddWithValue("@idAnimal", idAnimal);
        using var reader = command.ExecuteReader();

        if (!reader.Read()) return null;
        var animal = new Animal()
        {
            IdAnimal = (int)reader["IdAnimal"],
            Name = reader["Name"].ToString()!,
            Description = reader["Description"].ToString(),
            Category = reader["Category"].ToString()!,
            Area = reader["Area"].ToString()!,
        };
        return animal;
    }

    public Animal? CreateAnimal(string name, string? description, string category, string area)
    {
        using var connection = new SqlConnection(configuration["ConnectionStrings:DefaultConnection"]);
        connection.Open();

        using var command =
            new SqlCommand(
                "INSERT INTO Animal (name, description, category, area) OUTPUT INSERTED.* VALUES (@name, @description, @category, @area)",
                connection);
        command.Parameters.AddWithValue("@name", name);
        command.Parameters.AddWithValue("@category", category);
        command.Parameters.AddWithValue("@area", area);
        if (description != null)
        {
            command.Parameters.AddWithValue("@description", description);
        }
        else
        {
            command.Parameters.AddWithValue("@description", DBNull.Value);
        }

        var reader = command.ExecuteReader();

        if (!reader.Read()) return null;
        var animal = new Animal()
        {
            IdAnimal = (int)reader["IdAnimal"],
            Name = reader["Name"].ToString()!,
            Description = reader["Description"].ToString(),
            Category = reader["Category"].ToString()!,
            Area = reader["Area"].ToString()!,
        };
        return animal;
    }

    public Animal? UpdateAnimal(int idAnimal, string? name, string? description, string? category, string? area)
    {
        using var connection = new SqlConnection(configuration["ConnectionStrings:DefaultConnection"]);
        connection.Open();

        var sqlBuilder = new StringBuilder("UPDATE Animal SET ");
        var parameters = new List<SqlParameter>();

        if (name != null)
        {
            sqlBuilder.Append("name = @name");
            parameters.Add(new SqlParameter("@name", name));
        }

        if (description != null)
        {
            sqlBuilder.Append(", description = @description");
            parameters.Add(new SqlParameter("@description", description));
        }

        if (category != null)
        {
            sqlBuilder.Append(", category = @category");
            parameters.Add(new SqlParameter("@category", category));
        }

        if (area != null)
        {
            sqlBuilder.Append(", area = @area");
            parameters.Add(new SqlParameter("@area", area));
        }

        sqlBuilder.Append(" OUTPUT INSERTED.*");
        sqlBuilder.Append(" WHERE idAnimal = @idAnimal");
        parameters.Add(new SqlParameter("@idAnimal", idAnimal));

        var sql = sqlBuilder.ToString();

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddRange(parameters.ToArray());

        var reader = command.ExecuteReader();

        if (!reader.Read()) return null;
        var animal = new Animal()
        {
            IdAnimal = (int)reader["IdAnimal"],
            Name = reader["Name"].ToString()!,
            Description = reader["Description"].ToString(),
            Category = reader["Category"].ToString()!,
            Area = reader["Area"].ToString()!,
        };
        return animal;
    }


    public Animal? RemoveAnimal(int idAnimal)
    {
        using var connection = new SqlConnection(configuration["ConnectionStrings:DefaultConnection"]);
        connection.Open();

        using var command = new SqlCommand("DELETE FROM Animal OUTPUT DELETED.* WHERE idAnimal = @idAnimal", connection);
        command.Parameters.AddWithValue("@idAnimal", idAnimal);

        var reader = command.ExecuteReader();

        if (!reader.Read()) return null;
        var animal = new Animal()
        {
            IdAnimal = (int)reader["IdAnimal"],
            Name = reader["Name"].ToString()!,
            Description = reader["Description"].ToString(),
            Category = reader["Category"].ToString()!,
            Area = reader["Area"].ToString()!,
        };
        return animal;
    }
}