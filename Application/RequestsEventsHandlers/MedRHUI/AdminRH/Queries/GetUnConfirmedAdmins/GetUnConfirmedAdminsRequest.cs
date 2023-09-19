using Application.DTO.DataObjects;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using DAL.MainDB.Repositories.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.RequestsEventsHandlers.MedRHUI.AdminRH.Queries.GetUnConfirmedAdmins;

public class GetUnConfirmedAdminsHandler : IRequestHandler<GetUnConfirmedAdminsRequest, Result<List<AdminDO>>>
{
    private readonly ILogger<GetUnConfirmedAdminsHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IAdminRepository _adminRepository;
    private readonly IRoleRepository _roleRepository;

    public GetUnConfirmedAdminsHandler(ILogger<GetUnConfirmedAdminsHandler> logger, IMapper mapper, IAdminRepository adminRepository, IRoleRepository roleRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _adminRepository = adminRepository;
        _roleRepository = roleRepository;
    }

    public async Task<Result<List<AdminDO>>> Handle(GetUnConfirmedAdminsRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<List<AdminDO>>(IsSuccess: false);
        List<AdminDO> admins = new();

        var adminsEntity = await _adminRepository.AsQueryable(w => w.IsConfirmed != true || w.RoleUid == Guid.Empty).ToListAsync(cancellationToken);
        var rolesEntity = await _roleRepository.GetAllAsync(cancellationToken);
        var roles = _mapper.Map<List<RoleDO>>(rolesEntity);
        admins = _mapper.Map<List<AdminDO>>(adminsEntity);
        admins.ForEach(item =>
        {
            item.Role = roles.FirstOrDefault(w => w.Uid == item.RoleUid);
        });


        result.IsSuccess = true;
        result.Data = admins;
        return result;
    }
}

public class GetUnConfirmedAdminsRequest : RequestModel, IRequest<Result<List<AdminDO>>>
{
}
