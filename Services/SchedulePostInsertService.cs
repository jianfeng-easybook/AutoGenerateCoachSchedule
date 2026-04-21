using AutoGenerateCoachSchedule.Data;
using AutoGenerateCoachSchedule.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace AutoGenerateCoachSchedule.Services
{
    public class SchedulePostInsertService
    {
        private readonly ILogger<SchedulePostInsertService> _logger;
        private readonly CoachSeatRepository _coachSeatRepository;

        public SchedulePostInsertService(
            ILogger<SchedulePostInsertService> logger,
            CoachSeatRepository coachSeatRepository)
        {
            _logger = logger;
            _coachSeatRepository = coachSeatRepository;
        }

        public async Task HandleAsync(CoachSchedule row, PreparedScheduleRow prepared, SqlConnection conn, SqlTransaction tx, CancellationToken ct)
        {
            if (!row.Coach_ID.HasValue)
            {
                _logger.LogWarning("Skipping post insert behavior because Coach_ID is null. GUID={Guid}", row.GUID);
                return;
            }

            if (string.IsNullOrWhiteSpace(row.GUID))
            {
                _logger.LogWarning("Skipping post insert behavior because GUID is null or empty. Coach_ID={CoachId}", row.Coach_ID);
                return;
            }

            var insertedSeats = await _coachSeatRepository.InsertFromTemplateAsync(
                row.Coach_ID.Value,
                row.GUID,
                row.Create_User ?? "AutoGenerateCoachSchedule",
                conn,
                tx,
                ct);

            _logger.LogInformation(
                "Coach_Seat template clone completed. InsertedSeats={InsertedSeats}, GUID={Guid}, Coach_ID={CoachId}",
                insertedSeats,
                row.GUID,
                row.Coach_ID);
        }
    }
}
