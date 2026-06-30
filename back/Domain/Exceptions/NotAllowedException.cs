namespace Domain.Exceptions;

public class NotAllowedException : Exception
{
    public object? Payload { get; set; }

    public NotAllowedException(string message)
    {
        
    }
    public NotAllowedException(string message, object? payload) : base(message)
    {
        Payload = payload;
    }
}