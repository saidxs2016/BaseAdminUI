using Core.Services.CacheService.MicrosoftInMemory;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace UI.Extensions.Health
{
    public class HealthPublisher : IHealthCheckPublisher
    {
        private readonly ILogger<HealthPublisher> _logger;
        private readonly IMemoryCacheService _memoryCacheService;

        public HealthPublisher(ILogger<HealthPublisher> logger, IMemoryCacheService memoryCacheService)
        {
            _logger = logger;
            _memoryCacheService = memoryCacheService;
        }

        public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
