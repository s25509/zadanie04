namespace RestApi.Exceptions;

public class BadDateException : Exception
{
    public BadDateException(string message) : base(message)
    {
    }
}