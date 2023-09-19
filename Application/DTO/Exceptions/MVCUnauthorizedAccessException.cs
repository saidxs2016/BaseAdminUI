using Application.DTO.ResultType;

namespace Application.DTO.Exceptions;

public class MVCUnauthorizedAccessException : UnauthorizedAccessException
{
    public Result<object>? Result { get; set; }

    public MVCUnauthorizedAccessException(string message) : base(message) { }

    public MVCUnauthorizedAccessException(Result<object>? result, string? message = null) : base(message)
    {
        Result = result;
    }
}
