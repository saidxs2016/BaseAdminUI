using FluentValidation;
using MediatR.Pipeline;

namespace Application.RequestsEventsHandlers.MedBehaviors;

public class RequestFilter<TRequest> : IRequestPreProcessor<TRequest>
    where TRequest : class

{

    private readonly IValidator<TRequest> _validator;

    public RequestFilter(IValidator<TRequest> validator = null)
    {
        _validator = validator;
    }


    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
