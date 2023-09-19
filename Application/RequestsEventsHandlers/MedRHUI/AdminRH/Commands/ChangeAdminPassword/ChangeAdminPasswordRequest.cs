using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using Core.Security.Hashing;
using Core.Services.CacheService.MicrosoftInMemory;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.RequestsEventsHandlers.MedRHUI.AdminRH.Commands.ChangeAdminPassword;

public class ChangeAdminPasswordHandler : IRequestHandler<ChangeAdminPasswordRequest, Result<ResponseModel>>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IMapper _mapper;
    private readonly IMemoryCacheService _memoryCacheService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _uow;


    public ChangeAdminPasswordHandler(IAdminRepository adminRepository, IMapper mapper, IMemoryCacheService memoryCacheService, IHttpContextAccessor httpContextAccessor, IUnitOfWork uow)
    {
        _adminRepository = adminRepository;
        _mapper = mapper;
        _memoryCacheService = memoryCacheService;
        _httpContextAccessor = httpContextAccessor;
        _uow = uow;
    }

    public async Task<Result<ResponseModel>> Handle(ChangeAdminPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>(IsSuccess: false, Message: "Şifre değiştirme işlemi başarısız.", ResponseLogging: true);
        var model = new ResponseModel();

        var admin = await _adminRepository.GetAsFirstOrDefaultAsync(w => w.Uid == request.Uid, cancellationToken);
        HashingTool.CreatePasswordHash(request.Password, out byte[] hash, out byte[] salt);

        admin.PasswordHash = hash;
        admin.PasswordSalt = salt;
        _adminRepository.Update(admin);

        var update = await _uow.SaveChangesAsync();
        if (update > 0)
        {
            _memoryCacheService.RemoveByPattern($"\\.{request.Uid}\\.");

            result.IsSuccess = true;
            result.Message = "Şifre değiştirme işlemi başarılı";
        }
        return result;
    }
}
public class ChangeAdminPasswordRequest : IRequest<Result<ResponseModel>>
{
    public Guid? Uid { get; set; }
    public string Password { get; set; }
    public string RePassword { get; set; }

}
public class ChangeAdminPasswordValidator : AbstractValidator<ChangeAdminPasswordRequest>
{
    public ChangeAdminPasswordValidator()
    {
        RuleFor(p => p.Uid)
            .NotNull().WithMessage("Kullanıcı bulunamadı!")
            .NotEmpty().WithMessage("Kullanıcı bulunamadı!");
        RuleFor(p => p.Password)
            .NotNull().WithMessage("Şifre boş olmamalı")
            .NotEmpty().WithMessage("Şifre boş olmamalı");
        RuleFor(p => p.RePassword)
            .NotNull().WithMessage("Şifre boş olmamalı")
            .NotEmpty().WithMessage("Şifre boş olmamalı");
        RuleFor(p => p).Must(w => w.Password == w.RePassword).WithMessage("Şifreler eşleşmiyor!");


    }
}
