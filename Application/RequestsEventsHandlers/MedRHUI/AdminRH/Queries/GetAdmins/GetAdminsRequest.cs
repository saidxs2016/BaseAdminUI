using Application.DTO.DataObjects;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.RequestsEventsHandlers.MedRHUI.AdminRH.Queries.GetAdmins;

public class GetAdminsHandler : IRequestHandler<GetAdminsRequest, Result<List<AdminDO>>>
{
    private readonly ILogger<GetAdminsHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IAdminRepository _adminRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetAdminsHandler(ILogger<GetAdminsHandler> logger, IMapper mapper, IAdminRepository adminRepository, IRoleRepository roleRepository, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _mapper = mapper;
        _adminRepository = adminRepository;
        _roleRepository = roleRepository;
        _httpContextAccessor = contextAccessor;
    }

    public async Task<Result<List<AdminDO>>> Handle(GetAdminsRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<List<AdminDO>>(IsSuccess: false);

        //var roleName = _httpContextAccessor.HttpContext.User?.Claims.FirstOrDefault(i => i.Type == ClaimHelper.RoleName)?.Value;
        //var authId = _httpContextAccessor.HttpContext.User?.Claims.FirstOrDefault(i => i.Type == ClaimHelper.AuthID)?.Value;
        //var adminUid = Guid.Parse(authId.Split(".")[1]);

        var rolesEntities = await _roleRepository.GetAllAsync(cancellationToken);
        var roles = _mapper.Map<List<RoleDO>>(rolesEntities);

        //var superAdminRole = rolesEntities.FirstOrDefault(i => !i.ParentUid.HasValue);
        //if (!request.Uid.HasValue && superAdminRole.Name == roleName)
        //    request.Uid = superAdminRole.Uid;
        //else if (!request.Uid.HasValue)
        //    request.Uid = roles.FirstOrDefault(w => w.Name == roleName)?.Uid;

        var role = roles.FirstOrDefault(i => i.Uid == request.Uid);


        var adminsEntities = await _adminRepository.GetAsWhereAsync(i => i.RoleUid.Value == role.Uid);

        var admins = _mapper.Map<List<AdminDO>>(adminsEntities) ?? new List<AdminDO>();

        admins.ForEach(item =>
        {
            item.Role = roles.FirstOrDefault(w => w.Uid == item.RoleUid);
        });

        result.IsSuccess = true;
        result.Data = admins;

        return result;
    }
}

public class GetAdminsRequest : RequestModel, IRequest<Result<List<AdminDO>>>
{
    public Guid? Uid { get; set; } // role uid
}

public class GetAdminsValidator : AbstractValidator<GetAdminsRequest>
{
    //private ICoreLocalizer _loc = ServiceTool.ServiceProvider.GetService<ICoreLocalizer>();
    public GetAdminsValidator()
    {
        RuleFor(p => p.Uid)
            .NotNull().WithMessage("Bu alana erişiminiz bulunmamaktadır!")
            .NotEmpty().WithMessage("Bu alana erişiminiz bulunmamaktadır!");

    }
}
