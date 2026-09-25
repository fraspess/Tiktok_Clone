namespace Domain.Exceptions;

public class BadRequestException : ApiException
{
    public BadRequestException(string code, string? message = null) : base(code, message)
    {
    }
}
