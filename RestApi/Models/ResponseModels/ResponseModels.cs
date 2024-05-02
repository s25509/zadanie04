using System.ComponentModel;

namespace RestApi.Models.ResponseModels;

public class NotFoundResponseModel(string message)
{
    [DefaultValue(StatusCodes.Status404NotFound)]
    public int Status { get; } = StatusCodes.Status404NotFound;

    [DefaultValue("Product/Warehouse/Order with ID: <id> was not found")]
    public string Message { get; init; } = message;
}

public class NoChangesResponseModel(string message)
{
    [DefaultValue(StatusCodes.Status304NotModified)]
    public int Status { get; } = StatusCodes.Status304NotModified;

    [DefaultValue("Order with ID: <id> is already being processed")]
    public string Message { get; init; } = message;
}

public class BadRequestResponseModel(string message)
{
    [DefaultValue(StatusCodes.Status400BadRequest)]
    public int Status { get; } = StatusCodes.Status400BadRequest;

    [DefaultValue("Order with ID: <id> was created LATER than this request")]
    public string Message { get; init; } = message;
}

public class ConflictResponseModel(string message)
{
    [DefaultValue(StatusCodes.Status409Conflict)]
    public int Status { get; } = StatusCodes.Status409Conflict;

    [DefaultValue("Could not register a new Delivery with given parameters")]
    public string Message { get; init; } = message;
}

public class CreatedResponseModel(string message, Delivery delivery)
{
    [DefaultValue(StatusCodes.Status201Created)]
    public int Status { get; } = StatusCodes.Status201Created;

    [DefaultValue("Registered a new Delivery with ID: <id>")]
    public string Message { get; init; } = message;

    public Delivery Delivery { get; init; } = delivery;
}