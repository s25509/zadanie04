using System.ComponentModel;

namespace RestApi.Animals;

public class NotFoundResponseModel(string message)
{
    [DefaultValue(StatusCodes.Status404NotFound)]
    public int Status { get; } = StatusCodes.Status404NotFound;

    [DefaultValue("Animal with id: <id> not found")]
    public string Message { get; init; } = message;
}

public class ConflictResponseModel(string message)
{
    [DefaultValue(StatusCodes.Status409Conflict)]
    public int Status { get; } = StatusCodes.Status409Conflict;

    [DefaultValue("There was a problem with the operation")]
    public string Message { get; init; } = message;
}

public class BadRequestResponseModel(string message)
{
    [DefaultValue(StatusCodes.Status400BadRequest)]
    public int Status { get; } = StatusCodes.Status400BadRequest;

    [DefaultValue($"Cannot sort by: <non-existent-column>")]
    public string Message { get; init; } = message;
}

public class CreatedResponseModel(string message, Animal animal)
{
    [DefaultValue(StatusCodes.Status201Created)]
    public int Status { get; } = StatusCodes.Status201Created;

    [DefaultValue("Created a new Animal with id: <id>")]
    public string Message { get; init; } = message;

    public Animal Animal { get; init; } = animal;
}