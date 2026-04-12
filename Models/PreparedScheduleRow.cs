namespace AutoGenerateCoachSchedule.Models
{
    public class PreparedScheduleRow
    {
        public int AutoGenerateSchedules_ID { get; set; }

        public AutoGenerateSchedule SourceSchedule { get; set; } = null!;
        public AutoGenerateCoachScheduleTemplate SourceTemplate { get; set; } = null!;

        public DateTime TargetDate { get; set; }
        public DateTime DepartureDateTime { get; set; }

        public string? DuplicateRouteFromPlace { get; set; }
        public string? DuplicateRouteFromSubPlace { get; set; }
        public string? DuplicateRouteToPlace { get; set; }
        public string? DuplicateRouteToSubPlace { get; set; }
        public string? DuplicateSequenceGuid { get; set; }

        public int? SeatAvailability { get; set; }
    }
}
