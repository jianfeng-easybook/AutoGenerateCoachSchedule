using AutoGenerateCoachSchedule.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace AutoGenerateCoachSchedule.Services
{
    public class SchedulePostInsertService
    {
        private readonly ILogger<SchedulePostInsertService> _logger;

        public SchedulePostInsertService(ILogger<SchedulePostInsertService> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(CoachSchedule row, PreparedScheduleRow prepared, SqlConnection conn, SqlTransaction tx, CancellationToken ct)
        {
            _logger.LogDebug("Post insert hook executed for Template_ID={TemplateId}", prepared.SourceTemplate.Template_ID);
            return Task.CompletedTask;
        }
    }
}
