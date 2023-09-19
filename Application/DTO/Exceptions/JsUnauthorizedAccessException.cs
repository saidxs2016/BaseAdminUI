using Application.DTO.ResultType;

namespace Application.DTO.Exceptions;

public class JsUnauthorizedAccessException : UnauthorizedAccessException
{
    public Result<object>? Result { get; set; }

    public JsUnauthorizedAccessException(string message) : base(message) { }

    public JsUnauthorizedAccessException(Result<object>? result, string? message = null) : base(message)
    {
        Result = result;
    }
}
