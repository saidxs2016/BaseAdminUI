using Application.DTO.DataObjects;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using DAL.MainDB.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.RequestsEventsHandlers.MedRHUI.AdminRH.Queries.GetDeletedAdmins;

public class GetDeletedAdminsHandler : IRequestHandler<GetDeletedAdminsRequest, Result<List<AdminDO>>>
{
    private readonly ILogger<GetDeletedAdminsHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IAdminRepository _adminRepository;
    private readonly IRoleRepository _roleRepository;

    public GetDeletedAdminsHandler(ILogger<GetDeletedAdminsHandler> logger, IMapper mapper, IAdminRepository adminRepository, IRoleRepository roleRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _adminRepository = adminRepository;
        _roleRepository = roleRepository;
    }

    public Task<Result<List<AdminDO>>> Handle(GetDeletedAdminsRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<List<AdminDO>>(IsSuccess: false);
        //List<AdminDO> admins = new();

        //var adminsEntity = await _adminRepository.AsQueryable(w => w.IsDeleted == true).ToListAsync(cancellationToken);
        //var rolesEntity = await _roleRepository.GetAllAsync(cancellationToken);
        //var roles = _mapper.Map<List<RoleDO>>(rolesEntity);  
        //admins = _mapper.Map<List<AdminDO>>(adminsEntity);
        //admins.ForEach(item =>
        //{
        //    item.Role = roles.FirstOrDefault(w=> w.Uid == item.RoleUid);
        //});


        //result.IsSuccess = true;
        //result.Data = admins; 
        return Task.FromResult(result);
    }
}

public class GetDeletedAdminsRequest : RequestModel, IRequest<Result<List<AdminDO>>>
{
}
