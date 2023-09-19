using Application.SignalRHubs.Models;
using DAL.MainDB.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace Application.SignalRHubs.Hubs;

public class NotificationHub : Hub
{

    private readonly IUnitOfWork _uoW;
    private readonly IHttpContextAccessor _ctx;
    public NotificationHub(IUnitOfWork uoW, IHttpContextAccessor ctx)
    {
        _uoW = uoW;
        _ctx = ctx;
    }

    public async Task PushNotification(Model model)
    {

        

        //var res = await _clientExternal.Connection.InvokeAsync<bool>("PushNotification", model);
        await Clients.All.SendAsync("ReciveNotification", model);

    }

    [Authorize]
    public async Task<string> TestData(Model model)
    {
        var x = _ctx.HttpContext.Request.Method;
        await Task.Delay(200);
        return _ctx.HttpContext.User.Claims.FirstOrDefault(i => i.Type == ClaimHelper.FullName).Value;

    }



}

public class NotificationHubFilter : IHubFilter
{

    public ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        return next(invocationContext);
    }

    public Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
    {
        return next(context);
    }

    public Task OnDisconnectedAsync(HubLifetimeContext context, Exception? exception, Func<HubLifetimeContext, Exception?, Task> next)
    {
        return next(context, exception);
    }
}
