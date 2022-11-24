// -----------------------------------------------------------------------
//  <copyright file = "Program.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.ProAssistant.Api.Extensions;
using Prism.ProAssistant.Api.Middlewares;
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