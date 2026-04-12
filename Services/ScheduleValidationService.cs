using AutoGenerateCoachSchedule.Models;

namespace AutoGenerateCoachSchedule.Services
{
    public class ScheduleValidationService
    {
        public bool TryParseSeatAvailability(string? value, out int? seatAvailability)
        {
            seatAvailability = null;

            if (string.IsNullOrWhiteSpace(value)) return true;

            if (int.TryParse(value, out var parsed))
            {
                seatAvailability = parsed;
                return true;
            }

            return false;
        }

        public bool IsValid(AutoGenerateSchedule schedule, AutoGenerateCoachScheduleTemplate template, DateTime date)
        {
            if (schedule == null || template == null) return false;
            if (!schedule.RecurrenceType.HasValue) return false;
            if (string.IsNullOrWhiteSpace(template.FromPlace)) return false;
            if (string.IsNullOrWhiteSpace(template.ToPlace)) return false;
            if (!TryParseSeatAvailability(template.SeatAvailability, out _)) return false;
            return true;
        }
    }
}
