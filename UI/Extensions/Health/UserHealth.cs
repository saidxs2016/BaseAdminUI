using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace UI.Extensions.Health
{
    public class UserHealth : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var rnd = new Random().Next(1, 1000);
                if (rnd % 2 == 0)
                    return Task.FromResult(HealthCheckResult.Unhealthy("Service is Healthy, ben Said "));
                return Task.FromResult(HealthCheckResult.Healthy("Service is Healthy, ben Said "));
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(exception: ex));
            }

        }
    }
}
