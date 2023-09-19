using Microsoft.AspNetCore.SignalR;

namespace Application.SignalRHubs.Hubs;

public class GlobalHubFilter : IHubFilter
{
    public ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        var ctx = invocationContext.Context.GetHttpContext();
        if (ctx.User.Identity.IsAuthenticated)
            return next(invocationContext);

        return new();
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

