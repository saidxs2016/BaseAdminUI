using Application.DTO.DataObjects;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.RequestsEventsHandlers.MedRHUI.HelperRH.Queries.BreadCrumb;

public class BreadCrumbHandler : IRequestHandler<BreadCrumbRequest, Result<List<BreadCrumbHelper>>>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IModuleRepository _moduleRepository;
    private readonly IMapper _mapper;

    public BreadCrumbHandler(IHttpContextAccessor httpContextAccessor, IModuleRepository moduleRepository, IMapper mapper)
    {
        _httpContextAccessor = httpContextAccessor;
        _moduleRepository = moduleRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<BreadCrumbHelper>>> Handle(BreadCrumbRequest request, CancellationToken cancellationToken)
    {

        var result = new Result<List<BreadCrumbHelper>>(IsSuccess: false);

        var thisPagePath = _httpContextAccessor.HttpContext.Request.Headers.Referer;
        Uri uri = new(thisPagePath);

        var moduleEntities = await _moduleRepository.GetAllAsync(cancellationToken);
        var modules = _mapper.Map<List<ModuleDO>>(moduleEntities);
        var module = modules.FirstOrDefault(w => w.Type == ModuleHelper.Page && uri.LocalPath.ToLowerInvariant().StartsWith(w.Address.ToLower()));
        if (module == null)
            return result;

        var breadcrumbs = ModuleGrandParent(module, modules, new List<BreadCrumbHelper>());
        breadcrumbs.Reverse();

        result.IsSuccess = true;
        result.Data = breadcrumbs;
        return result;
    }

    public List<BreadCrumbHelper> ModuleGrandParent(ModuleDO module, List<ModuleDO> arr, List<BreadCrumbHelper> breadCrumbs)
    {
        var parent = arr.FirstOrDefault(i => i.Uid == module.ParentUid);

        breadCrumbs.Add(new BreadCrumbHelper
        {
            Text = module.Name,
            Slug = module.Address,
        });
        if (parent != null && parent.Uid != Guid.Empty)
            return ModuleGrandParent(parent, arr, breadCrumbs);
        else
            return breadCrumbs;

    }


}
public class BreadCrumbRequest : RequestModel, IRequest<Result<List<BreadCrumbHelper>>>
{
}

public class BreadCrumbValidator : AbstractValidator<BreadCrumbRequest>
{
    public BreadCrumbValidator()
    {
    }
}
