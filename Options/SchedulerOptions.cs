namespace AutoGenerateCoachSchedule.Options
{
    public class SchedulerOptions
    {
        public string RunUser { get; set; } = "AUTO_SCHEDULER";
        public List<DatabaseTarget> Databases { get; set; } = new List<DatabaseTarget>();
    }

    public class DatabaseTarget
    {
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public bool Enabled { get; set; }
        public string ConnectionString { get; set; } = string.Empty;
    }
}