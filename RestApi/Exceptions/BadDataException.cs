namespace RestApi.Exceptions;

public class BadDataException : Exception
{
    public BadDataException(string message) : base(message)
    {
    }
}