namespace Pulse.Shared.Results;

public class ApiResponse<T>
{
    public T? Result { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool Error { get; set; }

    public static ApiResponse<T> Success(T result, string message = "") => new()
    {
        Result = result,
        Message = message,
        Error = false
    };

    public static ApiResponse<T> Failure(string message) => new()
    {
        Result = default,
        Message = message,
        Error = true
    };
}