// -----------------------------------------------------------------------
//  <copyright file = "Program.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.ProAssistant.Api.Extensions;
using Prism.ProAssistant.Api.Middlewares;
using Prism.ProAssistant.Business;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

// Logs
SelfLog.Enable(Console.WriteLine);
var logBuilder = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {CorrelationId} {Level:u3}] {Message} (at {Caller}){NewLine}{Exception}")
    .WriteTo.File("./logs/pro-assistant.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
    .Enrich.FromLogContext()
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
        ModifyConnectionSettings = x => x.BasicAuthentication(Environment.GetEnvironmentVariable("ELK_INDEX_AUTH_USER"), Environment.GetEnvironmentVariable("ELK_INDEX_AUTH_PASSWORD")),
        FailureCallback = e => Console.WriteLine("Unable to submit event " + e.MessageTemplate),
        EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                           EmitEventFailureHandling.WriteToFailureSink |
                           EmitEventFailureHandling.RaiseCallback,
        InlineFields = true
    });
}

Log.Logger = logBuilder.CreateLogger();
builder.Host.UseSerilog();

// Add Mediatr
builder.Services.AddQueriesCommands();

// Add Mongo
builder.Services.AddDatabase();

// Add business services
builder.Services.AddBusinessServices();

// Add Cache
builder.Services.AddCache();

// Add Bearer
builder.Services.AddBearer();

// Add web stuff
builder.Services.AddHealthChecks();
builder.Services.AddControllers();

// Build and start app
var app = builder.Build();
app.UseMiddleware<ErrorLoggingMiddleware>();

app.UseCors(opt =>
{
    opt.AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins(
            EnvironmentConfiguration.GetMandatoryConfiguration("FRONT_DOMAIN_FQDN"),
            EnvironmentConfiguration.GetMandatoryConfiguration("FRONT_DOMAIN_CUSTOM")
        );
});

app.UseHealthChecks("/health");
app.MapControllers();

app.UseRouting();
app.UseAuthentication().UseAuthorization();

app.Run();