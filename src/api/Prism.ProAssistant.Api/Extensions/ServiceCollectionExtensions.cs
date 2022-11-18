// -----------------------------------------------------------------------
//  <copyright file = "ServiceCollectionExtensions.cs" company = "Prism">
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
using Prism.ProAssistant.Business;
using Prism.ProAssistant.Business.Behaviors;
using Prism.ProAssistant.Business.Commands;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Queries;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;
using Prism.ProAssistant.Documents.Generators;
using Prism.ProAssistant.Documents.Locales;

namespace Prism.ProAssistant.Api.Extensions
{
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
        
        public static void AddCache(this IServiceCollection services)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = EnvironmentConfiguration.GetMandatoryConfiguration("REDIS_CONNECTION_STRING");
                options.InstanceName = EnvironmentConfiguration.GetMandatoryConfiguration("ENVIRONMENT");
            });
        }
        
        public static void AddQueriesCommands(this IServiceCollection services)
        {
            var applicationAssembly = typeof(EntryPoint).Assembly;
            services.AddMediatR(new[]
            {
                applicationAssembly
            }, config => config.AsScoped());

            services.AddScoped<IRequestHandler<FindOne<Patient>, Patient?>, FindOneHandler<Patient>>();
            services.AddScoped<IRequestHandler<FindMany<Patient>, List<Patient>>, FindManyHandler<Patient>>();
            services.AddScoped<IRequestHandler<UpsertOne<Patient>, UpsertResult>, UpsertOneHandler<Patient>>();

            services.AddScoped<IRequestHandler<FindOne<Meeting>, Meeting?>, FindOneHandler<Meeting>>();
            services.AddScoped<IRequestHandler<FindMany<Meeting>, List<Meeting>>, FindManyHandler<Meeting>>();
            services.AddScoped<IRequestHandler<UpsertOne<Meeting>, UpsertResult>, UpsertOneHandler<Meeting>>();

            services.AddScoped<IRequestHandler<FindOne<Tariff>, Tariff?>, FindOneHandler<Tariff>>();
            services.AddScoped<IRequestHandler<FindMany<Tariff>, List<Tariff>>, FindManyHandler<Tariff>>();
            services.AddScoped<IRequestHandler<UpsertOne<Tariff>, UpsertResult>, UpsertOneHandler<Tariff>>();
            services.AddScoped<IRequestHandler<RemoveOne<Tariff>>, RemoveOneHandler<Tariff>>();
            
            services.AddScoped<IRequestHandler<FindOne<Document>, Document?>, FindOneHandler<Document>>();
            services.AddScoped<IRequestHandler<FindMany<Document>, List<Document>>, FindManyHandler<Document>>();
            services.AddScoped<IRequestHandler<UpsertOne<Document>, UpsertResult>, UpsertOneHandler<Document>>();
            services.AddScoped<IRequestHandler<RemoveOne<Document>, Unit>, RemoveOneHandler<Document>>();

            services.AddScoped<IRequestHandler<FindOne<Setting>, Setting?>, FindOneHandler<Setting>>();
            services.AddScoped<IRequestHandler<UpsertOne<Setting>, UpsertResult>, UpsertOneHandler<Setting>>();

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LogCommandsBehavior<,>));

            services.AddValidatorsFromAssembly(applicationAssembly);
        }

        public static void AddDatabase(this IServiceCollection services)
        {
            var mongoDbConnectionString = EnvironmentConfiguration.GetMandatoryConfiguration("MONGODB_CONNECTION_STRING");

            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard).WithRepresentation(BsonType.String));
            var client = new MongoClient(mongoDbConnectionString);
            var database = client.GetDatabase("proassistant");
            services.AddSingleton<IMongoClient>(client);
            services.AddSingleton(database);

            services.AddSingleton(new MongoDbConfiguration(mongoDbConnectionString));

            services.AddScoped<IOrganizationContext, OrganizationContext>();
        }

        public static void AddBusinessServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IUserContextAccessor, UserContextAccessor>();
            
            // Add documents services
            services.AddScoped<ILocalizator, Localizator>();
            services.AddScoped<IReceiptGenerator, ReceiptGenerator>();
        }
    }
}