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
        private readonly DatabaseTargetResolver _databaseTargetResolver;
        private readonly CoachScheduleFactory _factory;
        private readonly ScheduleValidationService _scheduleValidationService;
        private readonly PreparedScheduleBuilder _preparedScheduleBuilder;
        private readonly BatchDuplicateFilter _batchDuplicateFilter;
        private readonly SchedulePostInsertService _schedulePostInsertService;
        private readonly SchedulerOptions _options;
        private readonly ILogger<AutoGenerateService> _logger;

        public AutoGenerateService(
            AutoGenerateRepository autoGenerateRepository,
            CoachScheduleRepository coachScheduleRepository,
            GenerationWindowResolver generationWindowResolver,
            ExclusionEvaluator exclusionEvaluator,
            RecurrenceDueEvaluator recurrenceDueEvaluator,
            DatabaseTargetResolver databaseTargetResolver,
            CoachScheduleFactory factory,
            ScheduleValidationService scheduleValidationService,
            PreparedScheduleBuilder preparedScheduleBuilder,
            BatchDuplicateFilter batchDuplicateFilter,
            SchedulePostInsertService schedulePostInsertService,
            IOptions<SchedulerOptions> options,
            ILogger<AutoGenerateService> logger)
        {
            _autoGenerateRepository = autoGenerateRepository;
            _coachScheduleRepository = coachScheduleRepository;
            _generationWindowResolver = generationWindowResolver;
            _exclusionEvaluator = exclusionEvaluator;
            _recurrenceDueEvaluator = recurrenceDueEvaluator;
            _databaseTargetResolver = databaseTargetResolver;
            _factory = factory;
            _scheduleValidationService = scheduleValidationService;
            _preparedScheduleBuilder = preparedScheduleBuilder;
            _batchDuplicateFilter = batchDuplicateFilter;
            _schedulePostInsertService = schedulePostInsertService;
            _options = options.Value;
            _logger = logger;
        }

        public async Task ProcessAsync(CancellationToken cancellationToken)
        {
            var today = DateTime.Today;
            var databases = _databaseTargetResolver.Resolve();

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

            var targetDates = _generationWindowResolver.GetTargetDates(schedule.RecurrenceType, today)
                .Where(x => !_exclusionEvaluator.IsExcluded(x, schedule.ExcludedDays, schedule.ExcludedDates))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            if (targetDates.Count == 0)
            {
                _logger.LogInformation(
                    "Database={DatabaseName}, AutoGenerateSchedules_ID={Id} has no target dates after exclusions",
                    database.Name,
                    schedule.AutoGenerateSchedules_ID);
                return;
            }

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

                var preparedRows = new List<PreparedScheduleRow>();
                var validationFailures = 0;

                foreach (var targetDate in targetDates)
                {
                    foreach (var template in templates)
                    {
                        if (!_scheduleValidationService.IsValid(schedule, template, targetDate))
                        {
                            validationFailures++;
                            _logger.LogWarning(
                                "Database={DatabaseName}, AutoGenerateSchedules_ID={Id}, Template_ID={TemplateId}, TargetDate={TargetDate:yyyy-MM-dd} failed validation",
                                database.Name,
                                schedule.AutoGenerateSchedules_ID,
                                template.Template_ID,
                                targetDate);
                            continue;
                        }

                        preparedRows.Add(_preparedScheduleBuilder.Build(schedule, template, targetDate));
                    }
                }

                if (preparedRows.Count == 0)
                {
                    _logger.LogWarning(
                        "Database={DatabaseName}, AutoGenerateSchedules_ID={Id} has no prepared rows",
                        database.Name,
                        schedule.AutoGenerateSchedules_ID);

                    await tx.CommitAsync(cancellationToken);
                    return;
                }

                var dedupedRows = _batchDuplicateFilter.Filter(preparedRows);

                var inserted = 0;
                var skippedInBatch = preparedRows.Count - dedupedRows.Count;
                var skippedInDb = 0;

                foreach (var preparedRow in dedupedRows)
                {
                    var exists = await _coachScheduleRepository.ExistsAsync(
                        preparedRow.SourceTemplate.Coach_ID,
                        preparedRow.DepartureDateTime,
                        preparedRow.DuplicateRouteFromPlace,
                        preparedRow.DuplicateRouteFromSubPlace,
                        preparedRow.DuplicateRouteToPlace,
                        preparedRow.DuplicateRouteToSubPlace,
                        preparedRow.DuplicateSequenceGuid,
                        conn,
                        tx,
                        cancellationToken);

                    if (exists)
                    {
                        skippedInDb++;
                        continue;
                    }

                    var coachSchedule = _factory.Create(preparedRow, _options.RunUser);

                    await _coachScheduleRepository.InsertAsync(coachSchedule, conn, tx, cancellationToken);
                    await _schedulePostInsertService.HandleAsync(coachSchedule, preparedRow, conn, tx, cancellationToken);
                    inserted++;
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
                    "Completed Database={DatabaseName}, AutoGenerateSchedules_ID={Id}, Templates={Templates}, TargetDates={TargetDates}, Prepared={Prepared}, BatchDuplicateSkipped={BatchDuplicateSkipped}, DbDuplicateSkipped={DbDuplicateSkipped}, ValidationFailures={ValidationFailures}, Inserted={Inserted}",
                    database.Name,
                    schedule.AutoGenerateSchedules_ID,
                    templates.Count,
                    targetDates.Count,
                    preparedRows.Count,
                    skippedInBatch,
                    skippedInDb,
                    validationFailures,
                    inserted);
            }
            catch
            {
                await tx.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}