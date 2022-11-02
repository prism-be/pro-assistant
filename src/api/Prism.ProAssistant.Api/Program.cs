// -----------------------------------------------------------------------
//  <copyright file = "Program.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Prism.ProAssistant.Api.Graph;
using Prism.ProAssistant.Api.Graph.Meetings;
using Prism.ProAssistant.Api.Graph.Patients;
using Prism.ProAssistant.Api.Graph.Tariffs;
using Prism.ProAssistant.Api.Middlewares;
using Prism.ProAssistant.Business;
using Prism.ProAssistant.Business.Behaviors;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

// Logs
Serilog.Debugging.SelfLog.Enable(Console.WriteLine);
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
var applicationAssembly = typeof(EntryPoint).Assembly;
builder.Services.AddMediatR(new[]
{
    applicationAssembly
}, config => config.AsScoped());
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LogCommandsBehavior<,>));
builder.Services.AddValidatorsFromAssembly(applicationAssembly);

// Add Mongo
var mongoDbConnectionString = EnvironmentConfiguration.GetMandatoryConfiguration("MONGODB_CONNECTION_STRING");

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard).WithRepresentation(BsonType.String));
var client = new MongoClient(mongoDbConnectionString);
var database = client.GetDatabase("proassistant");
builder.Services.AddSingleton<IMongoClient>(client);
builder.Services.AddSingleton(database);

builder.Services.AddSingleton(new MongoDbConfiguration(mongoDbConnectionString));

builder.Services.AddScoped<IOrganizationContext, OrganizationContext>();

builder.Services.AddSingleton<LogExecutionDiagnosticEventListener>();

// GraphQL
builder.Services
    .AddGraphQLServer()
    .AddDiagnosticEventListener<LogExecutionDiagnosticEventListener>()
    .AddAuthorization()
    .AddQueryType(d => d.Name("Query"))
    .AddTypeExtension<MeetingQuery>()
    .AddTypeExtension<PatientQuery>()
    .AddTypeExtension<TariffQuery>()
    .AddMutationType(d => d.Name("Mutation"))
    .AddTypeExtension<MeetingMutation>()
    .AddTypeExtension<PatientMutation>()
    .AddTypeExtension<TariffMutation>()
    .AddType<MeetingType>()
    .AddType<PatientType>()
    .AddType<TariffType>()
    .AddMongoDbFiltering()
    .AddMongoDbSorting()
    .AddMongoDbProjections()
    .AddMongoDbPagingProviders();

// Add business services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContextAccessor, UserContextAccessor>();

// Add JWT
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(jwtOptions =>
    {
        jwtOptions.Authority = "https://byprism.b2clogin.com/byprism.onmicrosoft.com/B2C_1_PRO_ASSISTANT/v2.0/";
        jwtOptions.Audience = EnvironmentConfiguration.GetMandatoryConfiguration("AZURE_AD_CLIENT_ID");
        jwtOptions.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = AuthenticationFailed
        };
    });

Task AuthenticationFailed(AuthenticationFailedContext arg)
{
    // TODO : LOG
    return Task.FromResult(0);
}

// Add web stuff
builder.Services.AddHealthChecks();
builder.Services.AddControllers();

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
app.UseEndpoints(endpoints =>
{
    endpoints.MapGraphQL("/api/graphql");
});

app.Run();