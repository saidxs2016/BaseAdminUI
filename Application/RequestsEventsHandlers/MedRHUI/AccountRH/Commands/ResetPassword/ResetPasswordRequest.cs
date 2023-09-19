using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using Core.Security.Jwt;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Application.RequestsEventsHandlers.MedRHUI.AccountRH.Commands.ResetPassword;


public class ResetPasswordHandler : IRequestHandler<ResetPasswordRequest, Result<ResponseModel>>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;
    private readonly IJwtHelper _jwtHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _uow;

    public ResetPasswordHandler(IAdminRepository adminRepository, IMapper mapper, IWebHostEnvironment env, IJwtHelper jwtHelper, IHttpContextAccessor httpContextAccessor, IUnitOfWork uow)
    {
        _adminRepository = adminRepository;
        _mapper = mapper;
        _env = env;
        _jwtHelper = jwtHelper;
        _httpContextAccessor = httpContextAccessor;
        _uow = uow;
    }

    public async Task<Result<ResponseModel>> Handle(ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>(IsSuccess: false, ResponseLogging: true);
        var model = new ResponseModel();

        var (full_name, role_nme, user_name, auth_id, _, _, _) = _jwtHelper.ValidateToken(request.Token);
        if (string.IsNullOrEmpty(full_name) || string.IsNullOrEmpty(role_nme) || string.IsNullOrEmpty(user_name) || string.IsNullOrEmpty(auth_id))
        {
            var admin = await _adminRepository.GetAsFirstOrDefaultAsync(w => w.Token == request.Token, cancellationToken);
            if (admin != null)
            {
                admin.Token = null;
                admin.UpdateDate = DateTime.Now;

                _adminRepository.Update(admin);
                await _uow.SaveChangesAsync(cancellationToken);
            }


            _httpContextAccessor.HttpContext.Response.Redirect($"/ResetPasswordFailedPage?isSuccess=false&message={WebUtility.UrlEncode("Şifre sıfırlama linki süresi dolmuştur.")}");
            return result;
        }
        else
        {

            var admin = await _adminRepository.GetAsFirstOrDefaultAsync(w => w.Token == request.Token, cancellationToken);
            if (admin == null)
            {
                _httpContextAccessor.HttpContext.Response.Redirect($"/ResetPasswordFailedPage?isSuccess=false&message={WebUtility.UrlEncode("Bu link daha önce kullanlmıştır.")}");
                return result;
            }

            var adminUid = auth_id.Split(".")[1];
            var adminEntity = await _adminRepository.GetAsFirstOrDefaultAsync(w => w.Uid == Guid.Parse(adminUid) && w.Username == user_name, cancellationToken);

            if (adminEntity == null)
            {
                _httpContextAccessor.HttpContext.Response.Redirect($"/ResetPasswordFailedPage?isSuccess=false&message={WebUtility.UrlEncode("Geçersiz Link")}");
                return result;
            }

        }

        _httpContextAccessor.HttpContext.Response.Redirect($"/ResetPasswordPage?token={request.Token}");
        return result;
    }
}


public class ResetPasswordRequest : IRequest<Result<ResponseModel>>
{
    public string Token { get; set; }
}

public class ResetPasswordValidator : AbstractValidator<ResetPasswordRequest>
{
    //private ICoreLocalizer _loc = ServiceTool.ServiceProvider.GetService<ICoreLocalizer>();
    public ResetPasswordValidator()
    {
        RuleFor(p => p.Token)
            .NotNull().WithMessage("Doğrulama işlemi gerçekleştirilmedi.")
            .NotEmpty().WithMessage("Doğrulama işlemi gerçekleştirilmedi.");

    }
}
