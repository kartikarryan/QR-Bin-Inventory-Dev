namespace QrBin.Api.Exceptions;

public class AppException : Exception
{
    public int StatusCode { get; }
    public List<string> Errors { get; }

    public AppException(string message, int statusCode = 500, List<string>? errors = null)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors ?? [];
    }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message, 404) { }
}

public class ConflictException : AppException
{
    public ConflictException(string message) : base(message, 409) { }
}

public class AppValidationException : AppException
{
    public AppValidationException(string message, List<string>? errors = null)
        : base(message, 400, errors) { }
}

public class ForbiddenException : AppException
{
    public ForbiddenException(string message) : base(message, 403) { }
}

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message) : base(message, 401) { }
}
