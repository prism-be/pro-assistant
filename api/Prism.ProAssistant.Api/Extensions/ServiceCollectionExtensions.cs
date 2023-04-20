using Microsoft.AspNetCore.Authentication.JwtBearer;
using MongoDB.ApplicationInsights.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Prism.Core;
using Prism.Infrastructure.Authentication;
using Prism.Infrastructure.Providers;
using Prism.Infrastructure.Providers.Local;
using Prism.Infrastructure.Providers.Mongo;
using Prism.ProAssistant.Api.Config;
using Prism.ProAssistant.Api.Services;
using Prism.ProAssistant.Storage;
using Prism.ProAssistant.Storage.Events;
using Prism.ProAssistant.Storage.Users;

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

    public static void AddDatabase(this IServiceCollection services)
    {
        var mongoDbConnectionString = EnvironmentConfiguration.GetMandatoryConfiguration("MONGODB_CONNECTION_STRING");

        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard).WithRepresentation(BsonType.String));
        services.AddMongoClient(mongoDbConnectionString);
        services.AddSingleton(new MongoDbConfiguration(mongoDbConnectionString));
    }

    public static void AddProAssistant(this IServiceCollection services)
    {
        services.AddDistributedMemoryCache();

        services.AddScoped<IQueryService, QueryService>();
        services.AddScoped<IPdfService, PdfService>();
        services.AddScoped<IDataStorage, LocalStorage>();

        services.AddScoped<IUserOrganizationService, UserOrganizationService>();
        services.AddScoped<UserOrganization>(serviceProvider =>
        {
            var userOrganisationService = serviceProvider.GetService<IUserOrganizationService>();

            if (userOrganisationService == null)
            {
                throw new InvalidOperationException("The user organisation service is not registered.");
            }

            var organisation = userOrganisationService.GetUserOrganization();
            organisation.Wait(TimeSpan.FromSeconds(30));

            return new UserOrganization
            {
                Id = userOrganisationService.GetUserId() ?? Guid.Empty.ToString(),
                Organization = organisation.Result
            };
        });

        services.AddScoped<IEventStore, EventStore>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(EventStore).Assembly);
        });

        services.AddScoped<IGlobalStateProvider, MongoGlobalStateProvider>();
        services.AddScoped<IStateProvider, MongoStateProvider>();
    }
}