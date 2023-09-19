using Application.DTO.Exceptions;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using Core.Services.CacheService.MicrosoftInMemory;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.RequestsEventsHandlers.MedRHUI.RoleRH.Commands.RoleConstraintLogin;

public class RoleConstraintLoginHandler : IRequestHandler<RoleConstraintLoginRequest, Result<ResponseModel>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly IMemoryCacheService _memoryCacheService;
    private readonly IUnitOfWork _uow;

    public RoleConstraintLoginHandler(IRoleRepository adminRolRepository, IMapper mapper, IMemoryCacheService memoryCacheService, IUnitOfWork uow)
    {
        _roleRepository = adminRolRepository;
        _mapper = mapper;
        _memoryCacheService = memoryCacheService;
        _uow = uow;
    }

    public async Task<Result<ResponseModel>> Handle(RoleConstraintLoginRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>(IsSuccess: false, ResponseLogging: true);
        var model = new ResponseModel();

        var roleEntity = await _roleRepository.GetAsFirstOrDefaultAsync(w => w.Uid == request.Uid, cancellationToken);
        if (roleEntity == null)
        {
            var logResult = new Result<object>() { IsSuccess = false, ResultType = ResultTypeEnum.Warning, Data = request, Message = "Rol bulunamadı." };
            throw new ModelException(logResult);
        }
        if (roleEntity.LoginCount > request.LoginCount)
        {
            var getAllMemory = _memoryCacheService.GetByPattern($"^{roleEntity.Slug}\\.");
            if (getAllMemory.Count > request.LoginCount)
            {
                var keys = getAllMemory.Select(i => i.Key).ToList();
                List<string> target_keys = new();
                foreach (var key in keys)
                {
                    var splitedKey = key.ToString().Split(".");
                    var target_key = splitedKey[0] + "\\." + splitedKey[1];
                    target_keys.Add(target_key);
                }

                target_keys = target_keys.Distinct().ToList();
                foreach (var key in target_keys)
                {
                    var admins = _memoryCacheService.GetByPattern($"^{key}\\.");

                    if (admins != null && admins.Count > request.LoginCount)
                    {
                        var diff = admins.Count - request.LoginCount;
                        if (diff > 0)
                        {
                            var adminsOrder = admins.OrderBy(w => w.AbsoluteExpiration).ToArray();

                            for (int i = 0; i < diff; i++)
                                _memoryCacheService.Remove(adminsOrder[i].Key.ToString());
                        }
                    }
                }
            }
        }

        if (roleEntity.Expiration != request.Expiration)
            _memoryCacheService.RemoveByPattern($"^{roleEntity.Slug}\\.");

        roleEntity.LoginCount = request.LoginCount;
        roleEntity.Expiration = request.Expiration;
        roleEntity.UpdateDate = DateTime.Now;

        _roleRepository.Update(roleEntity);
        var update = await _uow.SaveChangesAsync();
        if (update > 0)
            return new Result<ResponseModel>(IsSuccess: true, Message: "Güncelleme işlemi başarılı.", ResponseLogging: true);
        else
            return new Result<ResponseModel>(IsSuccess: false, Message: "Güncelleme işlemi başarısız!.", ResponseLogging: true);

    }
}
public class RoleConstraintLoginRequest : IRequest<Result<ResponseModel>>
{
    public Guid Uid { get; set; }
    public int LoginCount { get; set; }
    public string Expiration { get; set; }

}
public class RoleConstraintLoginValidator : AbstractValidator<RoleConstraintLoginRequest>
{
    public RoleConstraintLoginValidator()
    {
        RuleFor(p => p.Uid)
            .NotNull().WithMessage("Kullanıcı bulunamadı!")
            .NotEmpty().WithMessage("Kullanıcı bulunamadı!");
        RuleFor(p => p.LoginCount)
            .NotNull().WithMessage("Minimum Oturum Açma Sayısı min. 1 Olmalı")
            .NotEmpty().WithMessage("Minimum Oturum Açma Sayısı min. 1 Olmalı")
            .Must(w => w >= 1).WithMessage("Minimum Oturum Açma Sayısı min. 1 Olmalı");
        RuleFor(p => p.Expiration)
            .NotNull().WithMessage("Minimum Oturum Açma Süresi min. 1 DK Olmalı")
            .NotEmpty().WithMessage("Minimum Oturum Açma Süresi min. 1 DK Olmalı");
    }
}
