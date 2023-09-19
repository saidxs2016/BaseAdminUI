using Application.DTO.ResultType;

namespace Application.DTO.Exceptions;

public class ModelException : Exception
{
    public Result<object>? Result { get; set; }

    public ModelException(string message) : base(message) { }

    public ModelException(Result<object>? result, string? message = null) : base(message)
    {
        Result = result;
    }
}
