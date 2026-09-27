namespace Domain.Exceptions;

public class NotAllowedException : ApiException
{
    public object? Payload { get; set; }

    public NotAllowedException(string code, string? message = null, object? payload = null) : base(code, message)
    {
        Payload = payload;
    }
}
