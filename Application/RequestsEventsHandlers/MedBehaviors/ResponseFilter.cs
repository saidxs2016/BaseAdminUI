using MediatR;
using MediatR.Pipeline;

namespace Application.RequestsEventsHandlers.MedBehaviors;

public class ResponseFilter<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    //where TResponse : IResult

{
    public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {

        return Task.CompletedTask;
    }
}
