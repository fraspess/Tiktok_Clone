namespace Domain.Exceptions;

public class UnauthorizedException : ApiException
{
    public UnauthorizedException(string code, string? message = null) : base(code, message)
    {
    }
}
