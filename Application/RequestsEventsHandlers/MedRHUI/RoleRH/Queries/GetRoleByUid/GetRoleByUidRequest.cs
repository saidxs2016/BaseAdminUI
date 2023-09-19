using Application.DTO.DataObjects;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using DAL.MainDB.Repositories.Interfaces;
using MediatR;

namespace Application.RequestsEventsHandlers.MedRHUI.RoleRH.Queries.GetRoleByUid;

public class GetRoleByUidHandler : IRequestHandler<GetRoleByUidRequest, Result<ResponseModel>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    public GetRoleByUidHandler(IRoleRepository adminRoleRepository, IMapper mapper)
    {
        _roleRepository = adminRoleRepository;
        _mapper = mapper;
    }

    public async Task<Result<ResponseModel>> Handle(GetRoleByUidRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>();
        var model = new ResponseModel();

        var data = await _roleRepository.GetAsFirstOrDefaultAsync(i => i.Uid == request.Uid);
        model.Role = _mapper.Map<RoleDO>(data);
        result.IsSuccess = true;
        result.Data = model;

        return result;
    }
}
public class GetRoleByUidRequest : RequestModel, IRequest<Result<ResponseModel>>
{
    public Guid? Uid { get; set; }
}
