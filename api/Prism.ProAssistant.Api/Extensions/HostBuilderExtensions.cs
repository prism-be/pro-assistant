using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Prism.ProAssistant.Api.Insights;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;

namespace Prism.ProAssistant.Api.Extensions;

public static class HostBuilderExtensions
{

    public static void AddApplicationInsights(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ITelemetryInitializer, RoleNameInitializer>();
        builder.Services.AddApplicationInsightsTelemetry();
        builder.Services.AddApplicationInsightsTelemetryProcessor<CleanTelemetryFilter>();
        builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
        builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft.Hosting.Lifetime", LogLevel.Information);
        builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Warning);
    }

    public static void AddSerilog(this WebApplicationBuilder builder)
    {
        SelfLog.Enable(Console.WriteLine);

        builder.Host.UseSerilog((_, services, configuration) =>
        {
            configuration
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {CorrelationId} {Level:u3}] {Message} (at {Caller}){NewLine}{Exception}")
                .Enrich.FromLogContext()
                .Enrich.WithClientIp()
                .Enrich.WithClientAgent()
                .Enrich.WithProperty("environment", Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "proassistant");

            var aiConnectionString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");

            if (!string.IsNullOrWhiteSpace(aiConnectionString))
            {
                var telemetryConfiguration = builder.Services.BuildServiceProvider().GetRequiredService<TelemetryConfiguration>();
                configuration.WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces);
            }
        });
    }
}