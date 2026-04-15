using AutoGenerateCoachSchedule.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace AutoGenerateCoachSchedule.Services
{
    public class DatabaseTargetResolver
    {
        private readonly SchedulerOptions _schedulerOptions;
        private readonly SqlServerOptions _sqlServerOptions;
        private readonly Dictionary<string, Dictionary<string, DatabaseEnvironmentOptions>> _databaseCatalog;

        public DatabaseTargetResolver(
            IOptions<SchedulerOptions> schedulerOptions,
            IOptions<SqlServerOptions> sqlServerOptions,
            IOptions<Dictionary<string, Dictionary<string, DatabaseEnvironmentOptions>>> databaseCatalogOptions)
        {
            _schedulerOptions = schedulerOptions.Value;
            _sqlServerOptions = sqlServerOptions.Value;
            _databaseCatalog = databaseCatalogOptions.Value;
        }

        public IList<DatabaseTarget> Resolve()
        {
            var results = new List<DatabaseTarget>();

            foreach (var country in _schedulerOptions.EnabledCountries)
            {
                if (!_databaseCatalog.ContainsKey(country))
                {
                    if (_schedulerOptions.FailOnMissingDatabaseMapping)
                        throw new InvalidOperationException("Missing database mapping for country: " + country);

                    continue;
                }

                var productMap = _databaseCatalog[country];

                foreach (var product in _schedulerOptions.EnabledProducts)
                {
                    if (!productMap.ContainsKey(product))
                    {
                        if (_schedulerOptions.FailOnMissingDatabaseMapping)
                            throw new InvalidOperationException(
                                string.Format("Missing database mapping for country '{0}', product '{1}'", country, product));

                        continue;
                    }

                    var envMap = productMap[product];
                    var environment = _schedulerOptions.TargetEnvironment;
                    var databaseName = string.Equals(environment, "Prod", StringComparison.OrdinalIgnoreCase)
                        ? envMap.Prod
                        : envMap.Test;

                    if (string.IsNullOrWhiteSpace(databaseName))
                    {
                        if (_schedulerOptions.FailOnMissingDatabaseMapping)
                            throw new InvalidOperationException(
                                string.Format("Database name missing for country '{0}', product '{1}', environment '{2}'",
                                    country, product, environment));

                        continue;
                    }

                    var builder = new SqlConnectionStringBuilder
                    {
                        DataSource = _sqlServerOptions.Server,
                        InitialCatalog = databaseName,
                        UserID = _sqlServerOptions.UserId,
                        Password = _sqlServerOptions.Password,
                        TrustServerCertificate = _sqlServerOptions.TrustServerCertificate
                    };

                    results.Add(new DatabaseTarget
                    {
                        Country = country,
                        Product = product,
                        Environment = environment,
                        Database = databaseName,
                        Name = country + "-" + product + "-" + environment,
                        ConnectionString = builder.ConnectionString
                    });
                }
            }

            return results;
        }
    }
}