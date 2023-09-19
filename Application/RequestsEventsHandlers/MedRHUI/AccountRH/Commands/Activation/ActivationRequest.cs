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

namespace Application.RequestsEventsHandlers.MedRHUI.AccountRH.Commands.Activation;

public class ActivationHandler : IRequestHandler<ActivationRequest, Result<ResponseModel>>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;
    private readonly IJwtHelper _jwtHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _uow;

    public ActivationHandler(IAdminRepository adminRepository, IMapper mapper, IWebHostEnvironment env, IJwtHelper jwtHelper, IHttpContextAccessor httpContextAccessor, IUnitOfWork uow)
    {
        _adminRepository = adminRepository;
        _mapper = mapper;
        _env = env;
        _jwtHelper = jwtHelper;
        _httpContextAccessor = httpContextAccessor;
        _uow = uow;
    }

    public async Task<Result<ResponseModel>> Handle(ActivationRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>(IsSuccess: false, ResponseLogging: true);
        var model = new ResponseModel();

        var (full_name, role_nme, user_name, auth_id, _, _, _) = _jwtHelper.ValidateToken(request.Token);
        if (string.IsNullOrEmpty(full_name) || string.IsNullOrEmpty(role_nme) || string.IsNullOrEmpty(user_name) || string.IsNullOrEmpty(auth_id))
        {
            var admin = await _adminRepository.GetAsFirstOrDefaultAsync(w => w.Token == request.Token, cancellationToken);
            if (admin != null)
            {
                admin.UpdateDate = DateTime.Now;
                _adminRepository.Delete(admin);
                await _uow.SaveChangesAsync(cancellationToken);
            }
            _httpContextAccessor.HttpContext.Response.Redirect($"/ActivationResult?isSuccess=false&message={WebUtility.UrlEncode("Aktivasyon Mailinizin Zamanı Geçmiş. Yeniden Kayıt Oluşturun!")}");
            return result;
            //return new Result<ResponseModel>(IsSuccess: false, Message: "Aktivasyon Mailinizin Zamanı Geçmiş.\n Yeniden Kayıt Oluşturun!", ResponseLogging: true);
        }
        else
        {
            var adminUid = auth_id.Split(".")[1];
            var admin = await _adminRepository.GetAsFirstOrDefaultAsync(w => w.Uid == Guid.Parse(adminUid) && w.Username == user_name, cancellationToken);
            if (admin == null)
            {
                _httpContextAccessor.HttpContext.Response.Redirect($"/ActivationResult?isSuccess=false&message={WebUtility.UrlEncode("Aktivasyon işlemi başarısız oldu!")}");
                return result;
            }
            if (admin.IsConfirmed == true)
            {
                _httpContextAccessor.HttpContext.Response.Redirect($"/ActivationResult?isSuccess=true&message={WebUtility.UrlEncode("Daha önce aktivasyon işlemi yapılmıştır.")}");
                return result;
            }

            admin.Token = null;
            admin.IsConfirmed = true;
            admin.UpdateDate = DateTime.Now;

            _adminRepository.Update(admin);
            await _uow.SaveChangesAsync(cancellationToken);
        }

        _httpContextAccessor.HttpContext.Response.Redirect($"/ActivationResult?isSuccess=true&message={WebUtility.UrlEncode("Aktivasyon işlemi gerçekleştirilmiştir.")}");
        return result;
    }
}


public class ActivationRequest : IRequest<Result<ResponseModel>>
{
    public string Token { get; set; }
}

public class ActivationValidator : AbstractValidator<ActivationRequest>
{
    //private ICoreLocalizer _loc = ServiceTool.ServiceProvider.GetService<ICoreLocalizer>();
    public ActivationValidator()
    {
        RuleFor(p => p.Token)
            .NotNull().WithMessage("Doğrulama işlemi gerçekleştirilmedi.")
            .NotEmpty().WithMessage("Doğrulama işlemi gerçekleştirilmedi.");

    }
}
