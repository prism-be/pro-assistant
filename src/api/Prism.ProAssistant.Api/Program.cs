// -----------------------------------------------------------------------
//  <copyright file = "Program.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Prism.ProAssistant.Business;
using Prism.ProAssistant.Business.Behaviors;

var builder = WebApplication.CreateBuilder(args);

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
app.UseAuthentication().UseAuthorization();
app.MapControllers();

app.Run();