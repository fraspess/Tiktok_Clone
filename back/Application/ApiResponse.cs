namespace Application;

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
    public IReadOnlyDictionary<string, string[]>? FieldErrors { get; set; }
    public string? Code { get; set; }

    public static ApiResponse<T> Success(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse<T> ErrorWithPayload(T data, string code)
    {
        return new ApiResponse<T>()
        {
            IsSuccess = false,
            Code = code,
            Data = data,
        };
    }
    public static ApiResponse<T> Error(string code, string? message = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            Code = code,
            Message = message,
            Errors = message != null ? [message] : null
        };
    }

    public static ApiResponse<T> ValidationError(IReadOnlyDictionary<string, string[]> fieldErrors, string? message = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            Message = message ?? "Validation failed",
            FieldErrors = fieldErrors
        };
    }
}