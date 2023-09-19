using FluentValidation;
using MediatR;

namespace Application.RequestsEventsHandlers.MedBehaviors;

public class MedPipelineFilter<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    //where TResponse : IResult

{

    private readonly IValidator<TRequest> _validator;

    public MedPipelineFilter(IValidator<TRequest> validator = null)
    {
        _validator = validator;
    }


    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            if (_validator is null)
                return await next();

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (validationResult.IsValid)
                return await next();

            throw new ValidationException(validationResult.Errors);

        }
        catch
        {
            throw;
        }
    }
}
