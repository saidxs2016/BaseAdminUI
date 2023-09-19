using Application.DTO.DataObjects;
using Application.DTO.Exceptions;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.RequestsEventsHandlers.MedRHUI.AdminRH.Queries.GetRoles;

public class GetRolesHandler : IRequestHandler<GetRolesRequest, Result<List<RoleDO>>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    public GetRolesHandler(IRoleRepository adminRoleRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _roleRepository = adminRoleRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<List<RoleDO>>> Handle(GetRolesRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<List<RoleDO>>();
        List<RoleDO> roles = new();

        if (_roleRepository.GetCount(w => true) > 0)
        {
            var roleEntity = await _roleRepository.GetAsFirstOrDefaultAsync(w => w.Uid == request.Uid, cancellationToken);
            if (roleEntity == null)
                throw new ModelException(new Result<object>(IsSuccess: false, Message: "Lütfen tekrar giriş yapınız!.", ResponseLogging: true));


            var rolesEntities = await _roleRepository.GetAllAsync(cancellationToken);
            roles = _mapper.Map<List<RoleDO>>(rolesEntities);

            var role = _mapper.Map<RoleDO>(roleEntity);

            role = RoleToTree(role, roles);
            roles = TreeToRole(role);
            roles.Add(role);

        }
        result.IsSuccess = true;
        result.Data = roles;

        return result;
    }

    public List<RoleDO> TreeToRole(RoleDO parent)
    {
        List<RoleDO> result = new();

        if (parent.SubRoleList != null && parent.SubRoleList.Count > 0)
            foreach (RoleDO t in parent.SubRoleList)
            {
                if (t.SubRoleList != null && t.SubRoleList.Count > 0)
                    result.AddRange(TreeToRole(t));
                result.Add(t);
            }
        return result;
    }
    public RoleDO RoleToTree(RoleDO parent, List<RoleDO> arr)
    {
        var tmp_arr = arr.Where(i => i.ParentUid == parent.Uid).ToList();
        if (tmp_arr != null && tmp_arr.Count > 0)
            foreach (RoleDO t in tmp_arr)
            {
                var sub_arr = arr.Where(i => i.ParentUid == t.Uid).ToList();
                if (sub_arr != null && sub_arr.Count > 0)
                    RoleToTree(t, arr);
                parent.SubRoleList ??= new List<RoleDO>();
                parent.SubRoleList.Add(t);
            }

        parent.SubRoleList ??= new List<RoleDO>();
        parent.SubRoleList = tmp_arr;
        return parent;
    }
}
public class GetRolesRequest : RequestModel, IRequest<Result<List<RoleDO>>>
{
    public Guid Uid { get; set; }
}

public class GetRolesRequestValidator : AbstractValidator<GetRolesRequest>
{
    //private ICoreLocalizer _loc = ServiceTool.ServiceProvider.GetService<ICoreLocalizer>();
    public GetRolesRequestValidator()
    {
        RuleFor(p => p.Uid)
            .NotNull().WithMessage("Bu alana erişiminiz bulunmamaktadır!")
            .Must(p => p != Guid.Empty).WithMessage("Bu alana erişiminiz bulunmamaktadır!");

    }
}
