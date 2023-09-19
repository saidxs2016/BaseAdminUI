using Application.DTO.DataObjects;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using DAL.MainDB.Repositories.Interfaces;
using MediatR;

namespace Application.RequestsEventsHandlers.MedRHUI.ModuleRH.Queries.GetModuleByUid;

public class GetModuleByModuleUidRequestHandler : IRequestHandler<GetModuleByUidRequest, Result<ResponseModel>>
{
    private readonly IModuleRepository _moduleRepository;
    private readonly IMapper _mapper;
    public GetModuleByModuleUidRequestHandler(IModuleRepository adminModuleRepository, IMapper mapper)
    {
        _moduleRepository = adminModuleRepository;
        _mapper = mapper;
    }

    public async Task<Result<ResponseModel>> Handle(GetModuleByUidRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>();
        var model = new ResponseModel();
        //var filter = new GlobalQueryFilter();

        //filter.AdminPredicate = i => i.Uid == request.Uid;

        var data = await _moduleRepository.GetAsFirstOrDefaultAsync(i => i.Uid == request.Uid);
        model.Module = _mapper.Map<ModuleDO>(data);
        result.IsSuccess = true;
        result.Data = model;

        return result;
    }
}
public class GetModuleByUidRequest : RequestModel, IRequest<Result<ResponseModel>>
{
    public Guid? Uid { get; set; }
}
