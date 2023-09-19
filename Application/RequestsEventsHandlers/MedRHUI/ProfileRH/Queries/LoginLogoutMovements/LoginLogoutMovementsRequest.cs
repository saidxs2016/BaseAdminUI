using Application.DTO.Models;
using Application.DTO.ResultType;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.RequestsEventsHandlers.MedRHUI.ProfileRH.Queries.LoginLogoutMovements;


public class LoginLogoutMovementsHandler : IRequestHandler<LoginLogoutMovementsRequest, Result<List<object>>>
{
    private readonly ILogger<LoginLogoutMovementsHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoginLogoutMovementsHandler(ILogger<LoginLogoutMovementsHandler> logger, IMapper mapper, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _mapper = mapper;
        _httpContextAccessor = contextAccessor;
    }

    public Task<Result<List<object>>> Handle(LoginLogoutMovementsRequest request, CancellationToken cancellationToken)
    {
        var result = new Result<List<object>>(IsSuccess: false);
        return Task.FromResult(result);

    }
}

public class LoginLogoutMovementsRequest : RequestModel, IRequest<Result<List<object>>>
{
}
