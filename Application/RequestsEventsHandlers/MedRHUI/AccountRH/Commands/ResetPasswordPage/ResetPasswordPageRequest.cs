using Application.DTO.Models;
using Application.DTO.ResultType;
using Core.Security.Hashing;
using Core.Security.Jwt;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.RequestsEventsHandlers.MedRHUI.AccountRH.Commands.ResetPasswordPage;

public class ResetPasswordPageHandler : IRequestHandler<ResetPasswordPageRequest, Result<ResponseModel>>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IJwtHelper _jwtHelper;
    private readonly IUnitOfWork _uow;

    public ResetPasswordPageHandler(IAdminRepository adminRepository, IJwtHelper jwtHelper, IUnitOfWork uow)
    {
        _adminRepository = adminRepository;
        _jwtHelper = jwtHelper;
        _uow = uow;
    }

    public async Task<Result<ResponseModel>> Handle(ResetPasswordPageRequest request, CancellationToken cancellationToken)
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
            return new Result<ResponseModel>(IsSuccess: false, Message: "Şifre Değiştirme Süresi Dolmuştur. Tekrar Şifremi Unuttum Yapabilirsiniz!.", ResponseLogging: true);
        }
        else
        {
            var adminUid = auth_id.Split(".")[1];
            var admin = await _adminRepository.GetAsFirstOrDefaultAsync(w => w.Uid == Guid.Parse(adminUid) && w.Username == user_name && w.Token == request.Token, cancellationToken);
            if (admin == null)
            {
                return new Result<ResponseModel>(IsSuccess: false, Message: "Şifre Değiştirme Süresi Dolmuştur. Tekrar Şifremi Unuttum Yapabilirsiniz!.", ResponseLogging: true);
            }

            HashingTool.CreatePasswordHash(request.Password, out byte[] hash, out byte[] salt);

            admin.PasswordHash = hash;
            admin.PasswordSalt = salt;
            admin.Token = null;
            admin.UpdateDate = DateTime.Now;

            _adminRepository.Update(admin);
            await _uow.SaveChangesAsync(cancellationToken);
        }

        return new Result<ResponseModel>(IsSuccess: true, Message: "Şifre değiştirilmiştir.", ResponseLogging: true);
    }
}


public class ResetPasswordPageRequest : IRequest<Result<ResponseModel>>
{
    public string? Token { get; set; }
    public string? Password { get; set; }
    public string? RePassword { get; set; }
}

public class ResetPasswordPageValidator : AbstractValidator<ResetPasswordPageRequest>
{
    //private ICoreLocalizer _loc = ServiceTool.ServiceProvider.GetService<ICoreLocalizer>();
    public ResetPasswordPageValidator()
    {
        RuleFor(p => p.Token)
            .NotNull().WithMessage("Doğrulama işlemi gerçekleştirilmedi.")
            .NotEmpty().WithMessage("Doğrulama işlemi gerçekleştirilmedi.");

        RuleFor(p => p.Password)
            .NotNull().WithMessage("Şifre boş olmamalı")
            .NotEmpty().WithMessage("Şifre boş olmamalı");
        RuleFor(p => p.RePassword)
            .NotNull().WithMessage("Şifre boş olmamalı")
            .NotEmpty().WithMessage("Şifre boş olmamalı");
        RuleFor(p => p).Must(w => w.Password == w.RePassword).WithMessage("Şifreler eşleşmiyor!");

    }
}
