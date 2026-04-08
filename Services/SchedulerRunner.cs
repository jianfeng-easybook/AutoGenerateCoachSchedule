using Microsoft.Extensions.Logging;

namespace AutoGenerateCoachSchedule.Services
{
    public class SchedulerRunner
    {
        private readonly AutoGenerateService _autoGenerateService;
        private readonly ILogger<SchedulerRunner> _logger;

        public SchedulerRunner(
            AutoGenerateService autoGenerateService,
            ILogger<SchedulerRunner> logger)
        {
            _autoGenerateService = autoGenerateService;
            _logger = logger;
        }

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Scheduler starting.");
            await _autoGenerateService.ProcessAsync(cancellationToken);
            _logger.LogInformation("Scheduler finished.");
        }
    }
}