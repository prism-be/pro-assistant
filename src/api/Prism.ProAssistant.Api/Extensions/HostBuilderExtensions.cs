// -----------------------------------------------------------------------
//  <copyright file = "HostBuilderExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Prism.ProAssistant.Api.Insights;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace Prism.ProAssistant.Api.Extensions;

public static class HostBuilderExtensions
{

    public static void AddApplicationInsights(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ITelemetryInitializer, RoleNameInitializer>();
        builder.Services.AddApplicationInsightsTelemetry();
        builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
        builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft.Hosting.Lifetime", LogLevel.Information);
        builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Warning);
    }

    public static void AddSerilog(this WebApplicationBuilder builder)
    {
        SelfLog.Enable(Console.WriteLine);
        var logBuilder = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {CorrelationId} {Level:u3}] {Message} (at {Caller}){NewLine}{Exception}")
            .Enrich.FromLogContext()
            .Enrich.WithCorrelationId()
            .Enrich.WithClientIp()
            .Enrich.WithClientAgent()
            .Enrich.WithProperty("environment", Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "proassistant");

        var elkUri = Environment.GetEnvironmentVariable("ELK_URI");

        if (!string.IsNullOrWhiteSpace(elkUri) && !builder.Environment.IsDevelopment())
        {
            logBuilder.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elkUri))
            {
                BatchAction = ElasticOpType.Create,
                TypeName = null,
                IndexFormat = Environment.GetEnvironmentVariable("ELK_INDEX_FORMAT"),
                ModifyConnectionSettings =
                    x => x.BasicAuthentication(Environment.GetEnvironmentVariable("ELK_INDEX_AUTH_USER"), Environment.GetEnvironmentVariable("ELK_INDEX_AUTH_PASSWORD")),
                FailureCallback = e => Console.WriteLine("Unable to submit event " + e.MessageTemplate),
                EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                                   EmitEventFailureHandling.WriteToFailureSink |
                                   EmitEventFailureHandling.RaiseCallback,
                InlineFields = true
            });
        }

        var aiConnectionString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");

        if (!string.IsNullOrWhiteSpace(aiConnectionString))
        {
            var telemetryConfiguration = builder.Services.BuildServiceProvider().GetRequiredService<TelemetryConfiguration>();
            logBuilder.WriteTo.ApplicationInsightsTraces(telemetryConfiguration);
        }

        Log.Logger = logBuilder.CreateLogger();
        builder.Host.UseSerilog();
    }
}