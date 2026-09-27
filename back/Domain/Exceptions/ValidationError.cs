namespace Domain.Exceptions;

public sealed record ValidationError(string PropertyName, string ErrorCode);