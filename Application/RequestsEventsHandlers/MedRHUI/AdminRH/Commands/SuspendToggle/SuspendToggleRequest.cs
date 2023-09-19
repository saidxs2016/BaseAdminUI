using Application.DTO.DataObjects;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using Core.Services.CacheService.MicrosoftInMemory;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.RequestsEventsHandlers.MedRHUI.AdminRH.Commands.SuspendToggle;

public class SuspendToggleHandler : IRequestHandler<SuspendToggleRequest, Result<ResponseModel>>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IMapper _mapper;
    private readonly IMemoryCacheService _memoryCacheService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _uow;

    public SuspendToggleHandler(IAdminRepository adminRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IMemoryCacheService memoryCacheService, IUnitOfWork uow)
    {
        _adminRepository = adminRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _memoryCacheService = memoryCacheService;
        _uow = uow;
    }

    public async Task<Result<ResponseModel>> Handle(SuspendToggleRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>(IsSuccess: false, ResponseLogging: true);
        var model = new ResponseModel();

        var admin = await _adminRepository.GetAsFirstOrDefaultAsync(w => w.Uid == request.Uid, cancellationToken);
        admin.IsSuspend = !admin.IsSuspend;
        admin.UpdateDate = DateTime.Now;

        _adminRepository.Update(admin);
        await _uow.SaveChangesAsync(cancellationToken);

        _memoryCacheService.RemoveByPattern($"\\.{admin.Uid}\\.");

        model.Admin = _mapper.Map<AdminDO>(admin);
        result.IsSuccess = true;
        result.Message = admin.IsSuspend == true ? "Askıya alma işlemi yapıldı" : "Askıdan çıkarma işlemi yapıldı";
        result.Data = model;
        return result;
    }
}
public class SuspendToggleRequest : IRequest<Result<ResponseModel>>
{
    public Guid? Uid { get; set; }

}
public class SuspendToggleValidator : AbstractValidator<SuspendToggleRequest>
{
    public SuspendToggleValidator()
    {
        RuleFor(p => p.Uid)
            .NotNull().WithMessage("Kullanıcı bulunamadı!")
            .NotEmpty().WithMessage("Kullanıcı bulunamadı!");


    }
}
