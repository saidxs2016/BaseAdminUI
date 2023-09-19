using Application.DTO.DataObjects;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using Core.Security.Hashing;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.RequestsEventsHandlers.MedRHUI.ProfileRH.Commands.GenerateConnectionKey;

public class GenerateConnectionKeyHandler : IRequestHandler<GenerateConnectionKeyRequest, Result<ResponseModel>>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _uow;
    public GenerateConnectionKeyHandler(IAdminRepository adminRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUnitOfWork uow)
    {
        _adminRepository = adminRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _uow = uow;
    }

    public async Task<Result<ResponseModel>> Handle(GenerateConnectionKeyRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>(IsSuccess: false, ResponseLogging: true);
        var model = new ResponseModel();

        var authId = _httpContextAccessor.HttpContext.User?.Claims.FirstOrDefault(i => i.Type == ClaimHelper.AuthID)?.Value;
        var adminUid = Guid.Parse(authId.Split(".")[1]);


        var admin = await _adminRepository.GetAsFirstOrDefaultAsync(w => w.Uid == adminUid, cancellationToken);


        if (!HashingTool.VerifyPasswordHash(request.Password, admin.PasswordHash, admin.PasswordSalt))
            return new Result<ResponseModel>(IsSuccess: false, Message: "Parola yanlış!", ResponseLogging: true);


        admin.UpdateDate = DateTime.Now;

        var connectionKey = new ConnectionKeyHelper()
        {
            Key = Guid.NewGuid(),
            Connected = new List<Guid>(),
            Description = request.Description,
            ValidTo = DateTime.Now.AddDays(1)
        };

        var connectionKeys = admin.ConnectionKeys?.DeserializeFromCamelCase<List<ConnectionKeyHelper>>() ?? new List<ConnectionKeyHelper>();
        connectionKeys.Add(connectionKey);
        admin.ConnectionKeys = connectionKeys.SerializeWithCamelCase();

        _adminRepository.Update(admin);
        await _uow.SaveChangesAsync(cancellationToken);

        model.Admin = _mapper.Map<AdminDO>(admin);
        result.IsSuccess = true;
        result.Message = "Key Oluşturuldu.";
        result.Data = model;
        return result;
    }
}
public class GenerateConnectionKeyRequest : IRequest<Result<ResponseModel>>
{
    public string Password { get; set; }
    public string Description { get; set; }

}
public class GenerateConnectionKeyValidator : AbstractValidator<GenerateConnectionKeyRequest>
{
    public GenerateConnectionKeyValidator()
    {
        RuleFor(p => p.Password)
            .NotNull().WithMessage("Şifre boş olmamalı")
            .NotEmpty().WithMessage("Şifre boş olmamalı");

    }
}
