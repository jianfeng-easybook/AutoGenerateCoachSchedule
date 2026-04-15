using AutoGenerateCoachSchedule.Data;
using AutoGenerateCoachSchedule.Options;
using AutoGenerateCoachSchedule.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog((context, services, configuration) =>
    {
        configuration.ReadFrom.Configuration(context.Configuration);
    })
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<SchedulerOptions>(context.Configuration.GetSection("Scheduler"));
        services.Configure<SqlServerOptions>(context.Configuration.GetSection("SqlServer"));
        services.Configure<Dictionary<string, Dictionary<string, DatabaseEnvironmentOptions>>>(
            context.Configuration.GetSection("DatabaseCatalog"));

        services.AddSingleton<AutoGenerateRepository>();
        services.AddSingleton<CoachScheduleRepository>();

        services.AddSingleton<GenerationWindowResolver>();
        services.AddSingleton<ExclusionEvaluator>();
        services.AddSingleton<RecurrenceDueEvaluator>();
        services.AddSingleton<CoachScheduleFactory>();
        services.AddSingleton<AutoGenerateService>();
        services.AddSingleton<SchedulerRunner>();
        services.AddSingleton<DatabaseTargetResolver>();
    })
    .Build();

try
{
    Log.Information("Application started.");

    var runner = host.Services.GetRequiredService<SchedulerRunner>();
    await runner.RunAsync();

    Log.Information("Application completed successfully.");
    return 0;
}
catch (Exception ex)
{
    Log.Error(ex, "Application failed.");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}