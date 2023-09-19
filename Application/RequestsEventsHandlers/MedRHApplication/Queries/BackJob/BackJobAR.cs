using Application.DTO.DataObjects;
using Application.DTO.Models;
using Application.DTO.ResultType;
using DAL.MainDB.Repositories.Interfaces;
using MediatR;

namespace Application.RequestsEventsHandlers.MedRHApplication.Queries.BackJob;

public class BackJobARH : IRequestHandler<BackJobAR, Result<ResponseModel>>
{
    private readonly IUnitOfWork _uow;


    public BackJobARH(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public Task<Result<ResponseModel>> Handle(BackJobAR request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>();
        var model = new ResponseModel();
        Console.WriteLine($" ++++++++++++ BackJobARHandler. {_uow.GetInstanceId()} ++++++++++++++");
        model.Admin = new AdminDO { Uid = _uow.GetInstanceId() };
        result.Data = model;



        return Task.FromResult(result);

    }
}

public record BackJobAR(bool satte) : IRequest<Result<ResponseModel>>;


