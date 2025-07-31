namespace HcAgents.Domain.Models.Base;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public Pagination? Pagination { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Ex { get; set; }

    public ApiResponse(bool success, int statusCode, string message, T? data)
    {
        Success = success;
        StatusCode = statusCode;
        Message = message;
        Data = data;
    }

    public ApiResponse(
        bool success,
        int statusCode,
        string message,
        string errorMessage,
        T? data = default
    )
    {
        Success = success;
        StatusCode = statusCode;
        Message = message;
        Data = data;
        ErrorMessage = errorMessage;
    }

    public ApiResponse(
        bool success,
        int statusCode,
        string message,
        string errorMessage,
        string ex,
        T? data = default
    )
    {
        Success = success;
        StatusCode = statusCode;
        Message = message;
        Data = data;
        ErrorMessage = errorMessage;
        Ex = ex;
    }

    public ApiResponse(
        bool success,
        int statusCode,
        string message,
        T? data,
        Pagination? pagination
    )
    {
        Success = success;
        StatusCode = statusCode;
        Message = message;
        Data = data;
        Pagination = pagination;
    }
}
