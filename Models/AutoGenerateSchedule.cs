namespace AutoGenerateCoachSchedule.Models
{
    public class AutoGenerateSchedule
    {
        public int AutoGenerateSchedules_ID { get; set; }
        public string? Schedule_GUID { get; set; }
        public string? SequenceGUID { get; set; }
        public int? Coach_ID { get; set; }
        public DateTime? From_Date { get; set; }
        public DateTime? To_Date { get; set; }
        public string? From_SubPlaceID { get; set; }
        public string? To_SubPlaceID { get; set; }
        public string? From_SubPlaceName { get; set; }
        public string? To_SubPlaceName { get; set; }
        public DateTime? Departure_Date { get; set; }
        public string? ExcludedDays { get; set; }
        public string? ExcludedDates { get; set; }
        public int? Status { get; set; }
        public DateTime? Create_Date { get; set; }
        public string? Create_User { get; set; }
        public DateTime? Update_Date { get; set; }
        public string? Update_User { get; set; }
        public DateTime? LastRunDate { get; set; }
        public int? RecurrenceType { get; set; }
    }
}