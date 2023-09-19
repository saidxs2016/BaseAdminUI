using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Application.CronJobs.HangfireFilter;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context)
    {
        var httpContext = context.GetHttpContext();


        if (httpContext.User != null && httpContext.User.Identity.IsAuthenticated && httpContext.User.Claims != null)
        {
            var role = httpContext.User.Claims.FirstOrDefault(i => i.Type == ClaimHelper.RoleName);
            if (!string.IsNullOrEmpty(role?.Value) && role.Value.ToLower() == "süper admin")
                return true;
        }

        return false;

    }
}
