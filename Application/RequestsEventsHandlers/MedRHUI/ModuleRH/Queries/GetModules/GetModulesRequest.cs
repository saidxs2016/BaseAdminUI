using Application.DTO.DataObjects;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using DAL.Extensions;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Application.RequestsEventsHandlers.MedRHUI.ModuleRH.Queries.GetModules;

public class GetModulesHandler : IRequestHandler<GetModulesRequest, Result<List<ModuleDO>>>
{
    private readonly IMapper _mapper;
    private readonly IModuleRepository _moduleRepository;
    private readonly IHttpContextAccessor _contextAccessor;

    public GetModulesHandler(IMapper mapper, IModuleRepository moduleRepository, IHttpContextAccessor contextAccessor)
    {
        _mapper = mapper;
        _moduleRepository = moduleRepository;
        _contextAccessor = contextAccessor;
    }

    public async Task<Result<List<ModuleDO>>> Handle(GetModulesRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<List<ModuleDO>>(IsSuccess: false);
        List<ModuleDO> modules = new();

        // await Task.Delay(1000);

        var Request = _contextAccessor.HttpContext.Request;
        var uidStr = Convert.ToString(Request.Form["uid"].FirstOrDefault());
        var start = Request.Form["start"].FirstOrDefault();
        var length = Request.Form["length"].FirstOrDefault();
        var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][data]"].FirstOrDefault();
        var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
        var searchValue = Request.Form["search[value]"].FirstOrDefault();

        _ = int.TryParse(length, out int pageSize);// sayfada kaç tane kayıt gösterilecek 
        _ = int.TryParse(start, out int skip);// sayfa numarası  
        _ = Guid.TryParse(uidStr, out Guid tmp_uid); // uid 

        Guid? uid = tmp_uid;
        if (uid == Guid.Empty)
            uid = null;


        Expression<Func<ModuleDO, bool>> ModulePredicate = w => w.ParentUid == uid;

        result = await GetData(request, ModulePredicate, cancellationToken);

        var modulesEntity = await _moduleRepository.AsQueryable().OrderBy(w => w.Order).ToListAsync(cancellationToken);
        modules = _mapper.Map<List<ModuleDO>>(modulesEntity);

        foreach (var item in result.Data)
            item.SubModuleList = modules.Where(w => w.ParentUid == item.Uid).ToList();

        result.IsSuccess = true;
        return result;
    }
    private async Task<Result<List<ModuleDO>>> GetData(GetModulesRequest request, Expression<Func<ModuleDO, bool>> modulePredicate, CancellationToken token = default)
    {
        var result = new Result<List<ModuleDO>>();

        var (query, data_count, filtered_data_count) = await BuildQueryAsync(request, modulePredicate, token);
        var props = _mapper.Map<PaginatedProps>(request);
        var data = await query.PaginatedRecordsAsync(props, token);

        result.Data = data;
        result.RecordsTotal = data_count;
        result.RecordsFiltered = !string.IsNullOrEmpty(request.SearchValue) ? filtered_data_count : data_count;

        return result;
    }
    private async Task<(IQueryable<ModuleDO>, long, long)> BuildQueryAsync(GetModulesRequest request, Expression<Func<ModuleDO, bool>> modulePredicate, CancellationToken token = default)
    {

        var query = from module in _moduleRepository.AsQueryable()
                    select new ModuleDO
                    {
                        Uid = module.Uid,
                        Action = module.Action,
                        AddDate = module.AddDate,
                        Address = module.Address,
                        Controller = module.Controller,
                        Icon = module.Icon,
                        IsMenu = module.IsMenu,
                        Name = module.Name,
                        Order = module.Order,
                        Type = module.Type,
                        ParentUid = module.ParentUid,
                        UpdateDate = module.UpdateDate
                    };

        //predicate
        if (modulePredicate != null)
            query = query.Where(modulePredicate);


        var data_count = await query.LongCountAsync(cancellationToken: token);

        // Search for English values 
        //if (!string.IsNullOrEmpty(request.SearchValue) && request.Lang == "en" )
        //

        // arama default tr
        if (!string.IsNullOrEmpty(request.SearchValue))
            query = query.Where(i => false
                    || (!string.IsNullOrEmpty(i.Controller) ? i.Controller.Contains(request.SearchValue) || i.Controller.ToLower().Contains(request.SearchValue.ToLower()) : false)
                    || (!string.IsNullOrEmpty(i.Action) ? i.Action.Contains(request.SearchValue) || i.Action.ToLower().Contains(request.SearchValue.ToLower()) : false)
                    || (!string.IsNullOrEmpty(i.Address) ? i.Address.Contains(request.SearchValue) || i.Address.ToLower().Contains(request.SearchValue.ToLower()) : false)
                    || (!string.IsNullOrEmpty(i.Name) ? i.Name.Contains(request.SearchValue) || i.Name.ToLower().Contains(request.SearchValue.ToLower()) || i.Name.ToUpper().Contains(request.SearchValue.ToLower()) : false)
            );

        var filtered_data_count = await query.LongCountAsync(cancellationToken: token);
        return (query, data_count, filtered_data_count);
    }
}

public class GetModulesRequest : RequestModel, IRequest<Result<List<ModuleDO>>>
{
    public Guid? Uid { get; set; }

}
