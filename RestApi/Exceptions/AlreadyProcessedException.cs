namespace RestApi.Exceptions;

public class AlreadyProcessedException : Exception
{
    public AlreadyProcessedException(string message) : base(message)
    {
    }
}