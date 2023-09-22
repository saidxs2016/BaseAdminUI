using Application.DTO.DataObjects;
using Application.DTO.Models;
using Application.DTO.ResultType;
using Application.RequestsEventsHandlers.MedRHUI.ModuleRH.Queries.GetModules;
using AutoMapper;
using DAL.Extensions;
using DAL.MainDB.Repositories.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Linq.Expressions;

namespace Application.RequestsEventsHandlers.MedRHUI.ModuleRH.Queries.GetAllModules;

public class GetAllModulesHandler : IRequestHandler<GetAllModulesRequest, Result<List<ModuleDO>>>
{
    private readonly ILogger<GetAllModulesHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IAdminRepository _adminRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly IHttpContextAccessor _contextAccessor;

    public GetAllModulesHandler(ILogger<GetAllModulesHandler> logger, IMapper mapper, IAdminRepository adminRepository, IModuleRepository moduleRepository, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _mapper = mapper;
        _adminRepository = adminRepository;
        _moduleRepository = moduleRepository;
        _contextAccessor = contextAccessor;
    }

    public async Task<Result<List<ModuleDO>>> Handle(GetAllModulesRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<List<ModuleDO>>(IsSuccess: false);
        List<ModuleDO> modules = new();



        var Request = _contextAccessor.HttpContext.Request;
        var start = Request.Form["start"].FirstOrDefault();
        var length = Request.Form["length"].FirstOrDefault();
        var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][data]"].FirstOrDefault();
        var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
        var searchValue = Request.Form["search[value]"].FirstOrDefault();

        _ = int.TryParse(length, out int pageSize);// sayfada kaç tane kayıt gösterilecek 
        _ = int.TryParse(start, out int skip);// sayfa numarası    

        Expression<Func<ModuleDO, bool>> ModulePredicate = i => true;

        var modulesEntity = await _moduleRepository.GetAllAsync(cancellationToken);
        modules = _mapper.Map<List<ModuleDO>>(modulesEntity);

        result = await GetData(modules, request, ModulePredicate);
        result.Data.ForEach(module =>
        {
            if (module.ParentUid != null)
                module.ParentModule = modules.FirstOrDefault(w => w.Uid == module.ParentUid);
            else
                module.ParentModule = new();

            module.SubModuleList = modules.Where(w => w.ParentUid == module.Uid).ToList();
        });


        result.IsSuccess = true;
        return result;
    }


    private async Task<Result<List<ModuleDO>>> GetData(List<ModuleDO> records, GetAllModulesRequest request, Expression<Func<ModuleDO, bool>> modulePredicate)
    {
        await Task.Delay(1);

        var result = new Result<List<ModuleDO>>();

        var (query, data_count, filtered_data_count) = BuildQuery(records, request, modulePredicate);
        var props = _mapper.Map<PaginatedProps>(request);
        var data = query.PaginatedRecords(props);


        result.Data = data;
        result.RecordsTotal = data_count;
        result.RecordsFiltered = !string.IsNullOrEmpty(request.SearchValue) ? filtered_data_count : data_count;

        return result;
    }

    private (IQueryable<ModuleDO>, long, long) BuildQuery(List<ModuleDO> records, GetAllModulesRequest request, Expression<Func<ModuleDO, bool>> modulePredicate)
    {
        var query = from module in records.AsQueryable()
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


        var data_count = query.LongCount();

        // Search for English values 
        //if (!string.IsNullOrEmpty(filter.SearchValue) && filter.Lang == "en" )
        //

        // arama default tr
        if (!string.IsNullOrEmpty(request.SearchValue))
            query = query.Where(i => false
                    || (!string.IsNullOrEmpty(i.Controller) ? i.Controller.Contains(request.SearchValue) || i.Controller.ToLower().Contains(request.SearchValue.ToLower()) : false)
                    || (!string.IsNullOrEmpty(i.Action) ? i.Action.Contains(request.SearchValue) || i.Action.ToLower().Contains(request.SearchValue.ToLower()) : false)
                    || (!string.IsNullOrEmpty(i.Address) ? i.Address.Contains(request.SearchValue) || i.Address.ToLower().Contains(request.SearchValue.ToLower()) : false)
                    || (!string.IsNullOrEmpty(i.Name) ? i.Name.Contains(request.SearchValue) || i.Name.ToLower().Contains(request.SearchValue.ToLower()) || i.Name.ToUpper().Contains(request.SearchValue.ToLower()) : false)
            );

        var filtered_data_count = query.LongCount();

        return (query, data_count, filtered_data_count);

    }
}

public class GetAllModulesRequest : RequestModel, IRequest<Result<List<ModuleDO>>>
{
}
