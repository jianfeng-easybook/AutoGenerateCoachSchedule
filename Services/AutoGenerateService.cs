using AutoGenerateCoachSchedule.Data;
using AutoGenerateCoachSchedule.Models;
using AutoGenerateCoachSchedule.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoGenerateCoachSchedule.Services
{
    public class AutoGenerateService
    {
        private readonly AutoGenerateRepository _autoGenerateRepository;
        private readonly CoachScheduleRepository _coachScheduleRepository;
        private readonly GenerationWindowResolver _generationWindowResolver;
        private readonly ExclusionEvaluator _exclusionEvaluator;
        private readonly RecurrenceDueEvaluator _recurrenceDueEvaluator;
        private readonly CoachScheduleFactory _factory;
        private readonly SchedulerOptions _options;
        private readonly ILogger<AutoGenerateService> _logger;

        public AutoGenerateService(
            AutoGenerateRepository autoGenerateRepository,
            CoachScheduleRepository coachScheduleRepository,
            GenerationWindowResolver generationWindowResolver,
            ExclusionEvaluator exclusionEvaluator,
            RecurrenceDueEvaluator recurrenceDueEvaluator,
            CoachScheduleFactory factory,
            IOptions<SchedulerOptions> options,
            ILogger<AutoGenerateService> logger)
        {
            _autoGenerateRepository = autoGenerateRepository;
            _coachScheduleRepository = coachScheduleRepository;
            _generationWindowResolver = generationWindowResolver;
            _exclusionEvaluator = exclusionEvaluator;
            _recurrenceDueEvaluator = recurrenceDueEvaluator;
            _factory = factory;
            _options = options.Value;
            _logger = logger;
        }

        public async Task ProcessAsync(CancellationToken cancellationToken)
        {
            var today = DateTime.Today;

            var databases = _options.Databases
                .Where(x => x.Enabled)
                .ToList();

            _logger.LogInformation("Enabled databases found: {Count}", databases.Count);

            foreach (var database in databases)
            {
                try
                {
                    _logger.LogInformation(
                        "Starting scheduler for database {Name} ({Country}/{Environment})",
                        database.Name,
                        database.Country,
                        database.Environment);

                    var schedules = await _autoGenerateRepository.GetActiveSchedulesAsync(
                        database.ConnectionString,
                        cancellationToken);

                    _logger.LogInformation(
                        "Database {Name}: Active schedules found: {Count}",
                        database.Name,
                        schedules.Count);

                    foreach (var schedule in schedules)
                    {
                        await ProcessScheduleAsync(database, schedule, today, cancellationToken);
                    }

                    _logger.LogInformation(
                        "Finished scheduler for database {Name}",
                        database.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Scheduler failed for database {Name} ({Country}/{Environment})",
                        database.Name,
                        database.Country,
                        database.Environment);
                }
            }
        }

        private async Task ProcessScheduleAsync(
            DatabaseTarget database,
            AutoGenerateSchedule schedule,
            DateTime today,
            CancellationToken cancellationToken)
        {
            if (!schedule.Departure_Date.HasValue)
            {
                _logger.LogWarning(
                    "Database={DatabaseName}, Skipping AutoGenerateSchedules_ID={Id} because Departure_Date is null",
                    database.Name,
                    schedule.AutoGenerateSchedules_ID);
                return;
            }

            if (!schedule.RecurrenceType.HasValue)
            {
                _logger.LogWarning(
                    "Database={DatabaseName}, Skipping AutoGenerateSchedules_ID={Id} because RecurrenceType is null",
                    database.Name,
                    schedule.AutoGenerateSchedules_ID);
                return;
            }

            var isDue = _recurrenceDueEvaluator.IsDue(
                schedule.RecurrenceType,
                schedule.LastRunDate,
                today);

            if (!isDue)
            {
                _logger.LogInformation(
                    "Database={DatabaseName}, AutoGenerateSchedules_ID={Id} is not due yet. LastRunDate={LastRunDate}, RecurrenceType={RecurrenceType}",
                    database.Name,
                    schedule.AutoGenerateSchedules_ID,
                    schedule.LastRunDate,
                    schedule.RecurrenceType);
                return;
            }

            var targetDates = _generationWindowResolver.GetTargetDates(schedule.RecurrenceType, today);
            if (targetDates.Count == 0)
                return;

            await using var conn = new SqlConnection(database.ConnectionString);
            await conn.OpenAsync(cancellationToken);

            await using var tx = (SqlTransaction)await conn.BeginTransactionAsync(cancellationToken);

            try
            {
                var templates = await _autoGenerateRepository.GetTemplatesAsync(
                    schedule.AutoGenerateSchedules_ID,
                    conn,
                    tx,
                    cancellationToken);

                if (templates.Count == 0)
                {
                    _logger.LogWarning(
                        "Database={DatabaseName}, No templates found for AutoGenerateSchedules_ID={Id}",
                        database.Name,
                        schedule.AutoGenerateSchedules_ID);

                    await tx.CommitAsync(cancellationToken);
                    return;
                }

                var inserted = 0;
                var skipped = 0;

                foreach (var targetDate in targetDates)
                {
                    if (_exclusionEvaluator.IsExcluded(targetDate, schedule.ExcludedDays, schedule.ExcludedDates))
                    {
                        skipped++;
                        continue;
                    }

                    foreach (var template in templates)
                    {
                        var finalDepartureDateTime = new DateTime(
                            targetDate.Year,
                            targetDate.Month,
                            targetDate.Day,
                            schedule.Departure_Date.Value.Hour,
                            schedule.Departure_Date.Value.Minute,
                            schedule.Departure_Date.Value.Second,
                            schedule.Departure_Date.Value.Millisecond);

                        var exists = await _coachScheduleRepository.ExistsAsync(
                            template.Coach_ID,
                            finalDepartureDateTime,
                            template.FromPlace,
                            template.FromSubPlace,
                            template.ToPlace,
                            template.ToSubPlace,
                            template.SequenceGUID,
                            conn,
                            tx,
                            cancellationToken);

                        if (exists)
                        {
                            skipped++;
                            continue;
                        }

                        var coachSchedule = _factory.Create(
                            template,
                            targetDate,
                            schedule.Departure_Date.Value,
                            _options.RunUser);

                        await _coachScheduleRepository.InsertAsync(coachSchedule, conn, tx, cancellationToken);
                        inserted++;
                    }
                }

                await _autoGenerateRepository.UpdateLastRunDateAsync(
                        schedule.AutoGenerateSchedules_ID,
                        today,
                        _options.RunUser,
                        conn,
                        tx,
                        cancellationToken);

                await tx.CommitAsync(cancellationToken);

                _logger.LogInformation(
                    "Completed Database={DatabaseName}, AutoGenerateSchedules_ID={Id}, Inserted={Inserted}, Skipped={Skipped}",
                    database.Name,
                    schedule.AutoGenerateSchedules_ID,
                    inserted,
                    skipped);
            }
            catch
            {
                await tx.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}