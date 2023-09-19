using Application.DTO.Models;
using Application.DTO.ResultType;
using FluentValidation;
using MediatR;

namespace Application.RequestsEventsHandlers.MedRHUI.AccountRH.Commands.Login;


public class LoginRequest : RequestModel, IRequest<Result<LoginResponse>>
{
    public string Username { get; set; }
    public string Password { get; set; }

}

public class LoginValidator : AbstractValidator<LoginRequest>
{
    //private ICoreLocalizer _loc = ServiceTool.ServiceProvider.GetService<ICoreLocalizer>();
    public LoginValidator()
    {

        RuleFor(p => p.Username)
            .NotNull().WithMessage("Kullanıcı adı veya şifre yanlış.")
            .NotEmpty().WithMessage("Kullanıcı adı veya şifre yanlış.")
            .MaximumLength(100).WithMessage("Kullanıcı adı veya şifre yanlış.");
        RuleFor(p => p.Password)
           .NotNull().WithMessage("Kullanıcı adı veya şifre yanlış.")
           .NotEmpty().WithMessage("Kullanıcı adı veya şifre yanlış.")
           .MaximumLength(50).WithMessage("Kullanıcı adı veya şifre yanlış.");

    }
}
