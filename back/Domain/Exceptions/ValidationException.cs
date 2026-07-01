namespace Domain.Exceptions;

public class ValidationException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationError> errors)
        : this()
    {
        Errors = errors
            .GroupBy(e => ToCamelCase(e.PropertyName), e => e.ErrorCode)
            .ToDictionary(g => g.Key, g => g.ToArray());
    }

    private static string ToCamelCase(string s) =>
        string.IsNullOrEmpty(s) ? s : char.ToLowerInvariant(s[0]) + s[1..];
}