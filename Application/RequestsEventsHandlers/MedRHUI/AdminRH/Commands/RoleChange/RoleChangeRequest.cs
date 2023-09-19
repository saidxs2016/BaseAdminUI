using Application.DTO.DataObjects;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using Core.Services.CacheService.MicrosoftInMemory;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Application.RequestsEventsHandlers.MedRHUI.AdminRH.Commands.RoleChange;

public class RoleChangeHandler : IRequestHandler<RoleChangeRequest, Result<ResponseModel>>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRoleRepository _roleRepository;
    private readonly IMemoryCacheService _memoryCacheService;
    private readonly IUnitOfWork _uow;

    public RoleChangeHandler(IAdminRepository adminRepository, IMapper mapper, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, IRoleRepository roleRepository, IMemoryCacheService memoryCacheService, IUnitOfWork uow)
    {
        _adminRepository = adminRepository;
        _mapper = mapper;
        _env = env;
        _httpContextAccessor = httpContextAccessor;
        _roleRepository = roleRepository;
        _memoryCacheService = memoryCacheService;
        _uow = uow;
    }

    public async Task<Result<ResponseModel>> Handle(RoleChangeRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>(IsSuccess: false, ResponseLogging: true);
        var model = new ResponseModel();

        var existAdmin = await _adminRepository.ExistAsync(w => w.Uid == request.Uid, cancellationToken);
        if (!existAdmin)
            return new Result<ResponseModel>(IsSuccess: false, Message: "Doğru veri göndermelisin!", ResponseLogging: true);

        var existRole = await _roleRepository.ExistAsync(w => w.Uid == request.RoleUid, cancellationToken);
        if (!existRole)
            return new Result<ResponseModel>(IsSuccess: false, Message: "Doğru veri göndermelisin!", ResponseLogging: true);


        var oldAdmin = await _adminRepository.GetAsFirstOrDefaultAsync(w => w.Uid == request.Uid, cancellationToken);

        oldAdmin.UpdateDate = DateTime.Now;
        oldAdmin.RoleUid = request.RoleUid;

        _adminRepository.Update(oldAdmin);
        var update = await _uow.SaveChangesAsync();
        if (update > 0)
        {
            result.Message = "Güncelleme işlemi başarılı.";
            _memoryCacheService.RemoveByPattern($"\\.{oldAdmin.Uid}\\.");
        }

        else
            result.Message = "Güncelleme işlemi başarısız!.";

        model.Role = new RoleDO { Uid = request.RoleUid.Value };

        result.IsSuccess = true;
        result.Data = model;

        return result;
    }
}


public class RoleChangeRequest : IRequest<Result<ResponseModel>>
{
    public Guid? Uid { get; set; }
    public Guid? RoleUid { get; set; }
}

public class RoleChangeValidator : AbstractValidator<RoleChangeRequest>
{
    public RoleChangeValidator()
    {
        RuleFor(p => p.Uid)
         .NotNull().WithMessage("Veriler doğru gelmedi!")
         .Must(w => w != Guid.Empty).WithMessage("Veriler doğru gelmedi!");
        RuleFor(p => p.RoleUid)
         .NotNull().WithMessage("Veriler doğru gelmedi!")
         .Must(w => w != Guid.Empty).WithMessage("Veriler doğru gelmedi!");

    }
}
