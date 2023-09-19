using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using DAL.MainDB.Repositories.Interfaces;
using MediatR;

namespace Application.RequestsEventsHandlers.MedRHUI.AdminRH.Commands.AdminDelete;

public class AdminDeleteHandler : IRequestHandler<AdminDeleteRequest, Result<ResponseModel>>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IMapper _mapper;

    public AdminDeleteHandler(IAdminRepository adminRepository, IMapper mapper)
    {
        _adminRepository = adminRepository;
        _mapper = mapper;
    }

    public Task<Result<ResponseModel>> Handle(AdminDeleteRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>();
        var model = new ResponseModel();



        return Task.FromResult(result);

    }
}



public class AdminDeleteRequest : IRequest<Result<ResponseModel>>
{

}
