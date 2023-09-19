using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using Core.Services.CacheService.MicrosoftInMemory;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.RequestsEventsHandlers.MedRHUI.HelperRH.Queries.HideIgnoreSection;

public class HideIgnoreSectionHandler : IRequestHandler<HideIgnoreSectionRequest, Result<List<string>>>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IMapper _mapper;
    private readonly IMemoryCacheService _memoryCacheService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HideIgnoreSectionHandler(IAdminRepository adminRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IMemoryCacheService memoryCacheService)
    {
        _adminRepository = adminRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _memoryCacheService = memoryCacheService;
    }

    public Task<Result<List<string>>> Handle(HideIgnoreSectionRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<List<string>>(IsSuccess: true);

        var authId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(w => w.Type == ClaimHelper.AuthID)?.Value;
        var authInfo = _memoryCacheService.Get<CacheAuthInfoHelper>(authId);

        var thisPagePath = _httpContextAccessor.HttpContext.Request.Headers.Referer;
        Uri uri = new(thisPagePath);


        if (authInfo == null || authInfo.Permissions == null)
            return Task.FromResult(new Result<List<string>>(IsSuccess: false, Message: "Yetkiniz yoktur, tekrar giriş yapınız."));

        List<string> ignores = new List<string>();

        var tmp_ignores = authInfo.Permissions.Where(w => uri.LocalPath.ToLowerInvariant().Contains(w.Module.Address.ToLowerInvariant())).Select(i => i.IgnoredSections).ToList();
        if (tmp_ignores != null)

            tmp_ignores.ForEach(i =>
            {
                var ignrs = i.DeserializeFromCamelCase<List<string>>();
                ignores.AddRange(ignrs);
            });


        result.IsSuccess = true;
        result.Data = ignores;
        return Task.FromResult(result);
    }

}
public class HideIgnoreSectionRequest : RequestModel, IRequest<Result<List<string>>>
{
    //public string? PageUrl { get; set; }
}
public class HideIgnoreSectionValidator : AbstractValidator<HideIgnoreSectionRequest>
{
    public HideIgnoreSectionValidator()
    {


    }
}
