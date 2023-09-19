using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Application.RequestsEventsHandlers.MedRHUI.ModuleRH.Commands.ModuleOrderRecords;

public class ModuleOrderRecordsHandler : IRequestHandler<ModuleOrderRecordsRequest, Result<ResponseModel>>
{
    private readonly IModuleRepository _moduleRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IUnitOfWork _uow;

    public ModuleOrderRecordsHandler(IModuleRepository adminModuleRepository, IMapper mapper, IHttpContextAccessor contextAccessor, IUnitOfWork uow)
    {
        _moduleRepository = adminModuleRepository;
        _mapper = mapper;
        _contextAccessor = contextAccessor;
        _uow = uow;
    }

    public async Task<Result<ResponseModel>> Handle(ModuleOrderRecordsRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>(IsSuccess: true, Message: "İşlem Başarılı");

        int order = 1;
        string[] uids = request.Uids.Select(w => $"\'{w}\'").ToArray();
        StringBuilder sb = new();
        foreach (string uid in uids)
            sb.Append($" when uid = {uid} then {order++} ");

        string sql = $"Update \"{request.Table}\" set \"{request.Column}\" = case {sb} end where uid in ({string.Join(',', uids)})";

        _ = await _uow.DbContext.Database.ExecuteSqlRawAsync(sql, cancellationToken: cancellationToken);

        return result;
    }
}

public class ModuleOrderRecordsRequest : IRequest<Result<ResponseModel>>
{
    //'table': table, 'column': column, 'idler': idler, 'indexler': indexler
    public string? Table { get; set; }
    public string? Column { get; set; }
    public string[]? Uids { get; set; }
}

public class ModuleOrderRecordsValidator : AbstractValidator<ModuleOrderRecordsRequest>
{
    //private ICoreLocalizer _loc = ServiceTool.ServiceProvider.GetService<ICoreLocalizer>();
    public ModuleOrderRecordsValidator()
    {

    }
}
