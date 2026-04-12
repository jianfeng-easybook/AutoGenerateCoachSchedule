using AutoGenerateCoachSchedule.Models;

namespace AutoGenerateCoachSchedule.Services
{
    public class PreparedScheduleBuilder
    {
        private readonly ScheduleValidationService _validation;

        public PreparedScheduleBuilder(ScheduleValidationService validation)
        {
            _validation = validation;
        }

        public PreparedScheduleRow Build(AutoGenerateSchedule schedule, AutoGenerateCoachScheduleTemplate template, DateTime targetDate)
        {
            if (!_validation.IsValid(schedule, template, targetDate))
                throw new InvalidOperationException("Invalid schedule/template");

            _validation.TryParseSeatAvailability(template.SeatAvailability, out var seat);

            var seed = schedule.Departure_Date ?? targetDate;

            var dt = new DateTime(targetDate.Year, targetDate.Month, targetDate.Day,
                seed.Hour, seed.Minute, seed.Second);

            return new PreparedScheduleRow
            {
                AutoGenerateSchedules_ID = schedule.AutoGenerateSchedules_ID,
                SourceSchedule = schedule,
                SourceTemplate = template,
                TargetDate = targetDate,
                DepartureDateTime = dt,
                DuplicateRouteFromPlace = template.FromPlace,
                DuplicateRouteFromSubPlace = template.FromSubPlace,
                DuplicateRouteToPlace = template.ToPlace,
                DuplicateRouteToSubPlace = template.ToSubPlace,
                DuplicateSequenceGuid = template.SequenceGUID,
                SeatAvailability = seat
            };
        }
    }
}
