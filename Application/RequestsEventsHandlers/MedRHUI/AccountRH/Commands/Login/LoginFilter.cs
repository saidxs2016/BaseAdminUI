using Application.DTO.ResultType;
using FluentValidation;
using MediatR;


namespace Application.RequestsEventsHandlers.MedRHUI.AccountRH.Commands.Login;



public class LoginFilter : IPipelineBehavior<LoginRequest, Result<LoginResponse>>
//where TResponse : IResult

{

    private readonly IValidator<LoginRequest> _validator;

    public LoginFilter(IValidator<LoginRequest> validator = null)
    {
        _validator = validator;
    }

    public async Task<Result<LoginResponse>> Handle(LoginRequest request, RequestHandlerDelegate<Result<LoginResponse>> next, CancellationToken cancellationToken)
    {
        try
        {

            return await next();

        }
        catch
        {
            throw;
        }

    }

}
