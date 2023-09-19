using Application.DTO.DataObjects;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using DAL.MainDB.Entities;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace Application.RequestsEventsHandlers.MedRHUI.ModuleRH.Commands.ModuleAdd;

public class ModuleAddHandler : IRequestHandler<ModuleAddRequest, Result<ResponseModel>>
{
    private readonly IModuleRepository _moduleRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IUnitOfWork _uow;

    public ModuleAddHandler(IModuleRepository adminModuleRepository, IMapper mapper, IHttpContextAccessor contextAccessor, IUnitOfWork uow)
    {
        _moduleRepository = adminModuleRepository;
        _mapper = mapper;
        _contextAccessor = contextAccessor;
        _uow = uow;
    }


    public async Task<Result<ResponseModel>> Handle(ModuleAddRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>(ResponseLogging: true);
        var model = new ResponseModel();

        if (request.Module.Type == ModuleHelper.Category && string.IsNullOrEmpty(request.Module.Controller))
            return new Result<ResponseModel>(IsSuccess: false, Message: "Controller adı zorunludur.");

        if (request.Module.Type != ModuleHelper.Category && string.IsNullOrEmpty(request.Module.Address))
            return new Result<ResponseModel>(IsSuccess: false, Message: "addres zorunludur.");

        var moduleExist = await _moduleRepository.ExistAsync(w => (w.Name == request.Module.Name || w.Name.ToLower() == request.Module.Name || w.Name.ToUpper() == request.Module.Name) && w.ParentUid == request.Module.ParentUid);
        if (moduleExist)
            return new Result<ResponseModel>(IsSuccess: false, Message: "Lütfen Başka Ad Deneyiniz!.");

        if (!string.IsNullOrEmpty(request.Module.Controller))
            request.Module.Controller = request.Module.Controller.ToLowerInvariant();

        if (!string.IsNullOrEmpty(request.Module.Action))
            request.Module.Action = request.Module.Action.ToLowerInvariant();

        if (!string.IsNullOrEmpty(request.Module.Address))
            request.Module.Address = request.Module.Address.ToLowerInvariant();

        Expression<Func<Module, bool>> ModulePredicate = w =>


                    w.Controller == request.Module.Controller

                &&

                    w.Action == request.Module.Action

                &&

                    w.Address == request.Module.Address

                &&

                    w.ParentUid == request.Module.ParentUid
                ;


        var moduleByControllerActionAddressExist = await _moduleRepository.ExistAsync(ModulePredicate);
        if (moduleByControllerActionAddressExist)
            return new Result<ResponseModel>(IsSuccess: false, Message: "Bu modul daha önce başka bir ad ile kullanılmıştır.");

        request.Module.Order = 9999;
        request.Module.AddDate = DateTime.Now;
        request.Module.UpdateDate = DateTime.Now;

        if (request.Module.ParentUid.HasValue && request.Module.ParentUid.Value == Guid.Empty)
            request.Module.ParentUid = null;

        await _moduleRepository.AddAsync(_mapper.Map<Module>(request.Module));
        var add = await _uow.SaveChangesAsync();
        if (add > 0)
            result.Message = "Kayıt işlemi başarılı.";
        else
            result.Message = "Kayıt işlemi başarısız!.";
        result.IsSuccess = true;

        return result;
    }
}

public class ModuleAddRequest : IRequest<Result<ResponseModel>>
{
    public ModuleDO? Module { get; set; }
}


public class ModuleAddValidator : AbstractValidator<ModuleAddRequest>
{
    //private ICoreLocalizer _loc = ServiceTool.ServiceProvider.GetService<ICoreLocalizer>();
    public ModuleAddValidator()
    {
        RuleFor(p => p.Module)
            .NotNull().WithMessage("Adam ol veriyi düzgün gir");
        RuleFor(p => p.Module.Name)
           .NotNull().WithMessage("Module adı boş olamaz!")
           .NotEmpty().WithMessage("Module adı boş olamaz!")
           .MinimumLength(2).WithMessage("En az 3 karakter girmelisiniz!.");
        RuleFor(p => p.Module.Type)
           .NotNull().WithMessage("Module tipi boş olamaz!")
           .NotEmpty().WithMessage("Module tipi boş olamaz!");

    }
}
