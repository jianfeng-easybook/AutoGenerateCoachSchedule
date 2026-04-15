namespace AutoGenerateCoachSchedule.Options
{
    public class SchedulerOptions
    {
        public string RunUser { get; set; } = "AUTO_SCHEDULER";
        public string TargetEnvironment { get; set; } = "Test";
        public List<string> EnabledCountries { get; set; } = new();
        public List<string> EnabledProducts { get; set; } = new();
        public bool ContinueOnDatabaseError { get; set; }
        public bool FailOnMissingDatabaseMapping { get; set; }
    }

    public class SqlServerOptions
    {
        public string Server { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool TrustServerCertificate { get; set; }
    }

    public class DatabaseCatalogOptions
    {
        public Dictionary<string, Dictionary<string, DatabaseEnvironmentOptions>> Catalog { get; set; } = new();
    }

    public class DatabaseEnvironmentOptions
    {
        public string Prod { get; set; } = string.Empty;
        public string Test { get; set; } = string.Empty;
    }

    public class DatabaseTarget
    {
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Product { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
    }
}