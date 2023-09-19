using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using Core.Security.Hashing;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.RequestsEventsHandlers.MedRHUI.ProfileRH.Commands.ChangePassword;

public class ChangePasswordHandler : IRequestHandler<ChangePasswordRequest, Result<ResponseModel>>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _uow;

    public ChangePasswordHandler(IAdminRepository adminRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUnitOfWork uow)
    {
        _adminRepository = adminRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _uow = uow;
    }

    public async Task<Result<ResponseModel>> Handle(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>(IsSuccess: false, ResponseLogging: true);
        var model = new ResponseModel();

        var authId = _httpContextAccessor.HttpContext.User?.Claims.FirstOrDefault(i => i.Type == ClaimHelper.AuthID)?.Value;
        var adminUid = Guid.Parse(authId.Split(".")[1]);


        var admin = await _adminRepository.GetAsFirstOrDefaultAsync(w => w.Uid == adminUid, cancellationToken);


        if (!HashingTool.VerifyPasswordHash(request.OldPassword, admin.PasswordHash, admin.PasswordSalt))
            return new Result<ResponseModel>(IsSuccess: false, Message: "Eski parola yanlış!", ResponseLogging: true);


        HashingTool.CreatePasswordHash(request.Password, out byte[] hash, out byte[] salt);

        admin.PasswordHash = hash;
        admin.PasswordSalt = salt;
        admin.UpdateDate = DateTime.Now;

        _adminRepository.Update(admin);
        await _uow.SaveChangesAsync(cancellationToken);


        result.IsSuccess = true;
        result.Message = "Şifre değiştirme işlemi başarılı";
        return result;
    }
}
public class ChangePasswordRequest : IRequest<Result<ResponseModel>>
{
    public string OldPassword { get; set; }
    public string Password { get; set; }
    public string RePassword { get; set; }

}
public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordValidator()
    {
        RuleFor(p => p.OldPassword)
            .NotNull().WithMessage("Eski Şifreyi Girmelisiniz!")
            .NotEmpty().WithMessage("Eski Şifreyi Girmelisiniz!");
        RuleFor(p => p.Password)
            .NotNull().WithMessage("Şifre boş olmamalı")
            .NotEmpty().WithMessage("Şifre boş olmamalı");
        RuleFor(p => p.RePassword)
            .NotNull().WithMessage("Şifre boş olmamalı")
            .NotEmpty().WithMessage("Şifre boş olmamalı");
        RuleFor(p => p).Must(w => w.Password == w.RePassword).WithMessage("Şifreler eşleşmiyor!");


    }
}
