namespace Domain.Exceptions;

public class NotFoundException : ApiException
{
    public NotFoundException(string code, string? message = null) : base(code, message)
    {
    }
}
