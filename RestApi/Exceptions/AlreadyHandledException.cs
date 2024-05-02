namespace RestApi.Exceptions;

public class AlreadyHandledException : Exception
{
    public AlreadyHandledException(string message) : base(message)
    {
    }
}