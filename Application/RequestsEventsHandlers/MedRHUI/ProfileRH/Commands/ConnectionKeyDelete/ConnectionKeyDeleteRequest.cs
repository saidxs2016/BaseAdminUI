using Application.DTO.DataObjects;
using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using DAL.MainDB.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.RequestsEventsHandlers.MedRHUI.ProfileRH.Commands.ConnectionKeyDelete;

public class ConnectionKeyDeleteHandler : IRequestHandler<ConnectionKeyDeleteRequest, Result<ResponseModel>>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _uow;

    public ConnectionKeyDeleteHandler(IAdminRepository adminRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUnitOfWork uow)
    {
        _adminRepository = adminRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _uow = uow;
    }

    public async Task<Result<ResponseModel>> Handle(ConnectionKeyDeleteRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<ResponseModel>(IsSuccess: false, ResponseLogging: true);
        var model = new ResponseModel();

        var authId = _httpContextAccessor.HttpContext.User?.Claims.FirstOrDefault(i => i.Type == ClaimHelper.AuthID)?.Value;
        var adminUid = Guid.Parse(authId.Split(".")[1]);


        var admin = await _adminRepository.GetAsFirstOrDefaultAsync(w => w.Uid == adminUid, cancellationToken);

        var connectionKeys = admin.ConnectionKeys?.DeserializeFromCamelCase<List<ConnectionKeyHelper>>() ?? new List<ConnectionKeyHelper>();
        var connectionKey = connectionKeys.FirstOrDefault(w => w.Key == request.Key && DateTime.Now > w.ValidTo);
        if (connectionKey == null)
            return new Result<ResponseModel>(IsSuccess: false, Message: "Zamanı henüz geçmemiş!", ResponseLogging: true);

        connectionKeys.Remove(connectionKey);

        admin.UpdateDate = DateTime.Now;
        admin.ConnectionKeys = connectionKeys.SerializeWithCamelCase();

        _adminRepository.Update(admin);
        await _uow.SaveChangesAsync(cancellationToken);

        model.Admin = _mapper.Map<AdminDO>(admin);
        result.IsSuccess = true;
        result.Message = "Bağlantı Anahtarı Silindi!";
        result.Data = model;
        return result;
    }
}
public class ConnectionKeyDeleteRequest : IRequest<Result<ResponseModel>>
{
    public Guid Key { get; set; }

}
public class ConnectionKeyDeleteValidator : AbstractValidator<ConnectionKeyDeleteRequest>
{
    public ConnectionKeyDeleteValidator()
    {
        RuleFor(p => p.Key)
            .NotNull().WithMessage("Seçim Yapmalısınız!")
            .NotEmpty().WithMessage("Seçim Yapmalısınız!");

    }
}
