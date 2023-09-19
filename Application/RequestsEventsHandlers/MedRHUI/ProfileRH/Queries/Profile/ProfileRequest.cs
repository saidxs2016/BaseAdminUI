using Application.DTO.DataObjects;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using Core.Services.CacheService.MicrosoftInMemory;
using DAL.MainDB.Repositories.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.RequestsEventsHandlers.MedRHUI.ProfileRH.Queries.Profile;


public class ProfileHandler : IRequestHandler<ProfileRequest, Result<ResponseModel>>
{
    private readonly ILogger<ProfileHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IAdminRepository _adminRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IMemoryCacheService _memoryCacheService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProfileHandler(ILogger<ProfileHandler> logger, IMapper mapper, IAdminRepository adminRepository, IRoleRepository roleRepository, IHttpContextAccessor contextAccessor, IMemoryCacheService memoryCacheService)
    {
        _logger = logger;
        _mapper = mapper;

        _adminRepository = adminRepository;
        _roleRepository = roleRepository;
        _httpContextAccessor = contextAccessor;
        _memoryCacheService = memoryCacheService;
    }

    public async Task<Result<ResponseModel>> Handle(ProfileRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>(IsSuccess: false);
        var model = new ResponseModel();


        var roleName = _httpContextAccessor.HttpContext.User?.Claims.FirstOrDefault(i => i.Type == ClaimHelper.RoleName)?.Value;
        var authId = _httpContextAccessor.HttpContext.User?.Claims.FirstOrDefault(i => i.Type == ClaimHelper.AuthID)?.Value;
        var adminUid = Guid.Parse(authId.Split(".")[1]);

        var role = await _roleRepository.GetAsFirstOrDefaultAsync(i => i.Name == roleName, cancellationToken);

        var adminsEntity = await _adminRepository.GetAsFirstOrDefaultAsync(i => i.Uid == adminUid);

        var admin = _mapper.Map<AdminDO>(adminsEntity);

        model.Admin = admin;
        model.Role = _mapper.Map<RoleDO>(role);
        result.IsSuccess = true;
        result.Data = model;

        return result;
    }
}

public class ProfileRequest : RequestModel, IRequest<Result<ResponseModel>>
{
    //public Guid? Uid { get; set; } // role uid
}
