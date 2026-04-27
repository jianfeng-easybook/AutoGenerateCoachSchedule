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
        private readonly CoachScheduleSequenceBuilder _coachScheduleSequenceBuilder;
        private readonly CoachScheduleSequenceRepository _coachScheduleSequenceRepository;

        public SchedulePostInsertService(
            ILogger<SchedulePostInsertService> logger,
            CoachSeatRepository coachSeatRepository,
            CoachScheduleSequenceBuilder coachScheduleSequenceBuilder,
            CoachScheduleSequenceRepository coachScheduleSequenceRepository)
        {
            _logger = logger;
            _coachSeatRepository = coachSeatRepository;
            _coachScheduleSequenceBuilder = coachScheduleSequenceBuilder;
            _coachScheduleSequenceRepository = coachScheduleSequenceRepository;
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

        public async Task HandleBatchAsync(
            IReadOnlyList<CoachSchedule> insertedRows,
            SqlConnection conn,
            SqlTransaction tx,
            CancellationToken ct)
        {
            if (insertedRows == null || insertedRows.Count == 0)
            {
                return;
            }

            var sequenceBatches = insertedRows
                .Where(x => !string.IsNullOrWhiteSpace(x.SequenceGUID))
                .GroupBy(x => x.SequenceGUID!, StringComparer.Ordinal);

            foreach (var batch in sequenceBatches)
            {
                var sequenceRows = _coachScheduleSequenceBuilder.Build(batch.ToList());
                if (sequenceRows.Count == 0)
                {
                    continue;
                }

                var insertedCount = await _coachScheduleSequenceRepository.ReplaceBySequenceGuidAsync(
                    batch.Key,
                    sequenceRows,
                    conn,
                    tx,
                    ct);

                _logger.LogInformation(
                    "CoachScheduleSequence upserted for SequenceGUID batch. SequenceGUID={SequenceGuid}, RowsInserted={RowsInserted}",
                    batch.Key,
                    insertedCount);
            }

            var scheduleBatches = insertedRows
                .Where(x => string.IsNullOrWhiteSpace(x.SequenceGUID) && !string.IsNullOrWhiteSpace(x.GUID))
                .GroupBy(x => x.GUID!, StringComparer.Ordinal);

            foreach (var batch in scheduleBatches)
            {
                var sequenceRows = _coachScheduleSequenceBuilder.Build(batch.ToList());
                if (sequenceRows.Count == 0)
                {
                    continue;
                }

                var insertedCount = await _coachScheduleSequenceRepository.ReplaceByScheduleGuidAsync(
                    batch.Key,
                    sequenceRows,
                    conn,
                    tx,
                    ct);

                _logger.LogInformation(
                    "CoachScheduleSequence upserted for ScheduleGUID batch. ScheduleGUID={ScheduleGuid}, RowsInserted={RowsInserted}",
                    batch.Key,
                    insertedCount);
            }
        }
    }
}
