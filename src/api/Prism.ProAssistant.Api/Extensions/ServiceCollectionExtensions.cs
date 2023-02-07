// -----------------------------------------------------------------------
//  <copyright file = "ServiceCollectionExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Authentication.JwtBearer;
using MongoDB.ApplicationInsights.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Prism.ProAssistant.Api.Insights;
using Prism.ProAssistant.Api.Security;
using Prism.ProAssistant.Business;
using Prism.ProAssistant.Business.Commands;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;
using Prism.ProAssistant.Business.Storage.Migrations;
using Prism.ProAssistant.Documents.Locales;

namespace Prism.ProAssistant.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddBearer(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
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
    }

    public static void AddBusinessServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<User>();

        services.AddTransient<OrganizationContextEnricher>();

        // Add documents services
        services.AddScoped<ILocalizator, Localizator>();
        
        services.AddScoped<IUpdateManyPropertyService, UpdateManyPropertyService>();
        services.AddScoped<IRemoveOneService, RemoveOneService>();
    }

    public static void AddCache(this IServiceCollection services)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = EnvironmentConfiguration.GetMandatoryConfiguration("REDIS_CONNECTION_STRING");
            options.InstanceName = EnvironmentConfiguration.GetMandatoryConfiguration("ENVIRONMENT");
        });
    }

    public static void AddDatabase(this IServiceCollection services)
    {
        var mongoDbConnectionString = EnvironmentConfiguration.GetMandatoryConfiguration("MONGODB_CONNECTION_STRING");

        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard).WithRepresentation(BsonType.String));
        services.AddMongoClient(mongoDbConnectionString);
        services.AddSingleton(new MongoDbConfiguration(mongoDbConnectionString));

        services.AddScoped<IOrganizationContext, OrganizationContext>();

        services.AddScoped<IMigrateDocumentConfiguration, MigrateDocumentConfiguration>();
    }
}