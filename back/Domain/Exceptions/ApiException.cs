using Domain.Constants;

namespace Domain.Exceptions;

public abstract class ApiException : Exception
{
    public string Code { get; }

    protected ApiException(string code, string? message = null)
        : base(message ?? ErrorMessages.For(code))
    {
        Code = code;
    }
}
