// -----------------------------------------------------------------------
//  <copyright file = "Program.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.ProAssistant.Api.Extensions;
using Prism.ProAssistant.Api.HealthChecks;
using Prism.ProAssistant.Api.Middlewares;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Workers;
using Prism.ProAssistant.Business;

var builder = WebApplication.CreateBuilder(args);

// Logs
builder.AddApplicationInsights();
builder.AddSerilog();

// Add Mediatr
builder.Services.AddQueriesCommands();

// Add Mongo
builder.Services.AddDatabase();

// Add business services
builder.Services.AddBusinessServices();

// Add Workers
builder.Services.AddWorkers();

// Add Cache
builder.Services.AddCache();

// Add Bearer
builder.Services.AddBearer();

// Add web stuff
builder.Services.AddHealthChecks()
    .AddCheck<CheckServiceBus>("Service Bus")
    .AddCheck<CheckCache>("Cache")
    .AddCheck<CheckDatabase>("Database");
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    options.SupportNonNullableReferenceTypes();
    options.SchemaFilter<RequiredNotNullableSchemaFilter>();
});

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

app.UseHealthChecks("/api/health");
app.MapControllers();

app.UseRouting();
app.UseAuthentication().UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();