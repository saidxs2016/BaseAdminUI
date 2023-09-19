using Application.DTO.DataObjects;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using DAL.MainDB.Repositories.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.RequestsEventsHandlers.MedRHUI.ModuleRH.Queries.GetDeletedModules;

public class GetDeletedModulesHandler : IRequestHandler<GetDeletedModulesRequest, Result<List<ModuleDO>>>
{
    private readonly ILogger<GetDeletedModulesHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IAdminRepository _adminRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly IHttpContextAccessor _contextAccessor;

    public GetDeletedModulesHandler(ILogger<GetDeletedModulesHandler> logger, IMapper mapper, IAdminRepository adminRepository, IModuleRepository moduleRepository, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _mapper = mapper;
        _adminRepository = adminRepository;
        _moduleRepository = moduleRepository;
        _contextAccessor = contextAccessor;
    }

    public Task<Result<List<ModuleDO>>> Handle(GetDeletedModulesRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<List<ModuleDO>>(IsSuccess: false);
        //List<ModuleDO> modules = new();

        //var modulesEntity = await _moduleRepository.AsQueryable(w => w.IsDeleted == true).ToListAsync(cancellationToken);

        //modules = _mapper.Map<List<ModuleDO>>(modulesEntity); 

        //result.IsSuccess = true;
        //result.Data = modules; 
        return Task.FromResult(result);
    }
}

public class GetDeletedModulesRequest : RequestModel, IRequest<Result<List<ModuleDO>>>
{
}
