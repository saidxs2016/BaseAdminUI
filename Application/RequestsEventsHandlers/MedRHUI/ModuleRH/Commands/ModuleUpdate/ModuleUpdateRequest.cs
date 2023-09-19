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

namespace Application.RequestsEventsHandlers.MedRHUI.ModuleRH.Commands.ModuleUpdate;

public class ModuleUpdateHandler : IRequestHandler<ModuleUpdateRequest, Result<ResponseModel>>
{
    private readonly IModuleRepository _moduleRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IUnitOfWork _uow;

    public ModuleUpdateHandler(IModuleRepository adminModuleRepository, IMapper mapper, IHttpContextAccessor contextAccessor, IUnitOfWork uow)
    {
        _moduleRepository = adminModuleRepository;
        _mapper = mapper;
        _contextAccessor = contextAccessor;
        _uow = uow;
    }

    public async Task<Result<ResponseModel>> Handle(ModuleUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>();
        var model = new ResponseModel();

        var moduleExist = await _moduleRepository.ExistAsync(w => w.Uid != request.Module.Uid && (w.Name == request.Module.Name || w.Name.ToLower() == request.Module.Name || w.Name.ToUpper() == request.Module.Name) && w.ParentUid == request.Module.ParentUid);
        if (moduleExist)
            return new Result<ResponseModel>(IsSuccess: false, Message: "Lütfen Başka Ad Deneyiniz!.");


        if (!string.IsNullOrEmpty(request.Module.Controller))
            request.Module.Controller = request.Module.Controller.ToLowerInvariant();

        if (!string.IsNullOrEmpty(request.Module.Action))
            request.Module.Action = request.Module.Action.ToLowerInvariant();

        if (!string.IsNullOrEmpty(request.Module.Address))
            request.Module.Address = request.Module.Address.ToLowerInvariant();

        Expression<Func<Module, bool>> ModulePredicate = w =>
                w.Uid != request.Module.Uid &&

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

        var oldData = await _moduleRepository.GetAsFirstOrDefaultAsync(w => w.Uid == request.Module.Uid);
        if (oldData == null)
            return new Result<ResponseModel>(IsSuccess: false, Message: "Güncellenecek kayıt tespit edilemedi!.");

        oldData.Name = request.Module.Name;
        oldData.Controller = request.Module.Controller;
        oldData.Action = request.Module.Action;
        oldData.Icon = request.Module.Icon;
        oldData.IsMenu = request.Module.IsMenu;
        oldData.Address = request.Module.Address;
        oldData.ParentUid = request.Module.ParentUid;
        if (request.Module.ParentUid.HasValue && request.Module.ParentUid.Value == Guid.Empty)
            oldData.ParentUid = null;
        oldData.Order = request.Module.Order;
        oldData.Type = request.Module.Type;


        oldData.UpdateDate = DateTime.Now;

        _moduleRepository.Update(oldData);
        var update = await _uow.SaveChangesAsync();
        if (update > 0)
            result.Message = "Güncelleme işlemi başarılı.";
        else
            result.Message = "Güncelleme işlemi başarısız!.";
        result.IsSuccess = true;

        return result;
    }
}

public class ModuleUpdateRequest : IRequest<Result<ResponseModel>>
{
    public ModuleDO? Module { get; set; }
}


public class ModuleUpdateValidator : AbstractValidator<ModuleUpdateRequest>
{
    //private ICoreLocalizer _loc = ServiceTool.ServiceProvider.GetService<ICoreLocalizer>();
    public ModuleUpdateValidator()
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
