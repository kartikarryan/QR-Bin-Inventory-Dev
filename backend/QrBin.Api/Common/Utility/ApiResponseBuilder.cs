using QrBin.ViewModels;

namespace QrBin.Api.Common.Utility;

public interface IApiResponseBuilder
{
    ApiResponse<T> Ok<T>(T? data, string? message = null);
    ApiResponse<T> Created<T>(T? data, string? message = null);
    ApiResponse<T> NoContent<T>(string? message = null);
    ApiResponse<T> NotFound<T>(string? message = null);
    ApiResponse<T> BadRequest<T>(T? data, string? message = null, List<string>? errors = null);
    ApiResponse<T> Conflict<T>(string? message = null);
    ApiResponse<T> Unauthorized<T>(string? message = null);
    ApiResponse<T> InternalServerError<T>(string? message = null, List<string>? errors = null);
}

public class ApiResponseBuilder : IApiResponseBuilder
{
    public ApiResponse<T> Ok<T>(T? data, string? message = null) => new()
    {
        StatusCode = 200,
        Message = message ?? "Success",
        Data = data
    };

    public ApiResponse<T> Created<T>(T? data, string? message = null) => new()
    {
        StatusCode = 201,
        Message = message ?? "Created",
        Data = data
    };

    public ApiResponse<T> NoContent<T>(string? message = null) => new()
    {
        StatusCode = 204,
        Message = message ?? "No Content"
    };

    public ApiResponse<T> NotFound<T>(string? message = null) => new()
    {
        StatusCode = 404,
        Message = message ?? "Not Found"
    };

    public ApiResponse<T> BadRequest<T>(T? data, string? message = null, List<string>? errors = null) => new()
    {
        StatusCode = 400,
        Message = message ?? "Bad Request",
        Data = data,
        Errors = errors
    };

    public ApiResponse<T> Conflict<T>(string? message = null) => new()
    {
        StatusCode = 409,
        Message = message ?? "Conflict"
    };

    public ApiResponse<T> Unauthorized<T>(string? message = null) => new()
    {
        StatusCode = 401,
        Message = message ?? "Unauthorized"
    };

    public ApiResponse<T> InternalServerError<T>(string? message = null, List<string>? errors = null) => new()
    {
        StatusCode = 500,
        Message = message ?? "Internal Server Error",
        Errors = errors
    };
}
