using Application.DTO.Models;
using Application.DTO.ResultType;
using Core.Services.CacheService.MicrosoftInMemory;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.RequestsEventsHandlers.MedRHUI.AccountRH.Commands.Logout;

public class LogoutHandler : IRequestHandler<LogoutRequest, Result<ResponseModel>>
{
    private readonly IMemoryCacheService _memoryCacheService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LogoutHandler(IMemoryCacheService memoryCacheService, IHttpContextAccessor httpContextAccessor)
    {
        _memoryCacheService = memoryCacheService;
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<Result<ResponseModel>> Handle(LogoutRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>(IsSuccess: false, ResponseLogging: true);
        var isLogin = _httpContextAccessor.HttpContext.User?.Identity.IsAuthenticated ?? false;
        if (!isLogin)
            return Task.FromResult(result);

        var authId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(w => w.Type == ClaimHelper.AuthID).Value;
        if (_memoryCacheService.IsAdd(authId))
            _memoryCacheService.Remove(authId);

        _httpContextAccessor.HttpContext.Response.Cookies.Delete("Access-Token");
        _httpContextAccessor.HttpContext.User = null;

        return Task.FromResult(new Result<ResponseModel>(IsSuccess: true, Message: "Çıkış Yaptı.", ResponseLogging: true));
    }


}

public class LogoutRequest : IRequest<Result<ResponseModel>>
{

}

public class LogoutValidator : AbstractValidator<LogoutRequest>
{
    public LogoutValidator()
    {

        //RuleFor(p => p.AccountInfo.)
        //    .NotNull().WithMessage("Kullanıcı adı veya şifre yanlış.")
        //    .NotEmpty().WithMessage("Kullanıcı adı veya şifre yanlış.")
        //    .MaximumLength(100).WithMessage("Kullanıcı adı veya şifre yanlış.");
        //RuleFor(p => p.Password)
        //   .NotNull().WithMessage("Kullanıcı adı veya şifre yanlış.")
        //   .NotEmpty().WithMessage("Kullanıcı adı veya şifre yanlış.")
        //   .MaximumLength(50).WithMessage("Kullanıcı adı veya şifre yanlış.");

    }
}
