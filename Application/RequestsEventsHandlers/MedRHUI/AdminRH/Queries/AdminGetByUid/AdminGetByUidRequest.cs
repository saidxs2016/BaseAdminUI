using Application.DTO.DataObjects;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.RequestsEventsHandlers.MedRHUI.AdminRH.Queries.AdminGetByUid;

public class AdminGetByUidRequestHandler : IRequestHandler<AdminGetByUidRequest, Result<ResponseModel>>
{
    private readonly IRoleRepository _adminRoleRepository;
    private readonly IMapper _mapper;
    private readonly IAdminRepository _adminRepository;
    public AdminGetByUidRequestHandler(IRoleRepository adminRoleRepository, IMapper mapper, IAdminRepository adminRepository)
    {
        _adminRoleRepository = adminRoleRepository;
        _mapper = mapper;
        _adminRepository = adminRepository;
    }

    public async Task<Result<ResponseModel>> Handle(AdminGetByUidRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>();
        var model = new ResponseModel();

        var admin = await _adminRepository.GetAsFirstOrDefaultAsync(i => i.Uid == request.Uid);

        model.Admin = _mapper.Map<AdminDO>(admin);
        result.IsSuccess = true;
        result.Data = model;

        return result;
    }
}

public class AdminGetByUidRequest : RequestModel, IRequest<Result<ResponseModel>>
{
    public Guid? Uid { get; set; }
}

public class AdminGetByUidValidator : AbstractValidator<AdminGetByUidRequest>
{
    //private ICoreLocalizer _loc = ServiceTool.ServiceProvider.GetService<ICoreLocalizer>();
    public AdminGetByUidValidator()
    {
        RuleFor(p => p.Uid).NotNull().WithMessage("Kullanıcı bulunamadı!");
        RuleFor(p => p.Uid).NotEmpty().WithMessage("Kullanıcı bulunamadı!");
    }
}
