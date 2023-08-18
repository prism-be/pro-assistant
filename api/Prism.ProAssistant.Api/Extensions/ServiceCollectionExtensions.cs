using Microsoft.AspNetCore.Authentication.JwtBearer;
using MongoDB.ApplicationInsights.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Prism.Core;
using Prism.Infrastructure.Authentication;
using Prism.Infrastructure.Providers;
using Prism.Infrastructure.Providers.Azure;
using Prism.Infrastructure.Providers.Local;
using Prism.Infrastructure.Providers.Mongo;
using Prism.ProAssistant.Api.Config;
using Prism.ProAssistant.Api.Services;
using Prism.ProAssistant.Storage;
using Prism.ProAssistant.Storage.Events;
using Prism.ProAssistant.Storage.Users;

namespace Prism.ProAssistant.Api.Extensions;

using Domain;
using Domain.Accounting.Document;
using Domain.Accounting.Forecast;
using Domain.Configuration.DocumentConfiguration;
using Domain.Configuration.Settings;
using Domain.Configuration.Tariffs;
using Domain.DayToDay.Appointments;
using Domain.DayToDay.Contacts;
using Microsoft.Extensions.Azure;
using QuestPDF.Infrastructure;
using Storage.Effects;

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
        
        if (!string.IsNullOrWhiteSpace(EnvironmentConfiguration.GetConfiguration("AZURE_STORAGE_CONNECTION_STRING")))
        {
            services.AddScoped<IDataStorage, BlobDataStorage>();
        }
        else
        {
            services.AddScoped<IDataStorage, MongoDataStorage>();
        }
        
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
                Organization = organisation.Result,
                Name = userOrganisationService.GetName() ?? string.Empty
            };
        });

        services.AddScoped<IEventStore, EventStore>();
        services.AddScoped<IHydrator, EventStore>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(EventStore).Assembly);
        });

        services.AddScoped<IGlobalStateProvider, MongoGlobalStateProvider>();
        services.AddScoped<IStateProvider, MongoStateProvider>();
        
        var serviceBusConnectionString = EnvironmentConfiguration.GetConfiguration("AZURE_SERVICE_BUS_CONNECTION_STRING");

        if (!string.IsNullOrWhiteSpace(serviceBusConnectionString))
        {
            services.AddAzureClients(builder => { builder.AddServiceBusClient(serviceBusConnectionString); });
            services.AddScoped<IPublisher, ServiceBusPublisher>();
        }

        services.AddTransient<IDomainAggregator<Appointment>, AppointmentAggregator>();
        services.AddTransient<IDomainAggregator<Contact>, ContactAggregator>();
        services.AddTransient<IDomainAggregator<DocumentConfiguration>, DocumentConfigurationAggregator>();
        services.AddTransient<IDomainAggregator<Setting>, SettingAggregator>();
        services.AddTransient<IDomainAggregator<Tariff>, TariffAggregator>();
        services.AddTransient<IDomainAggregator<Forecast>, ForecastAggregator>();
        services.AddTransient<IDomainAggregator<AccountingDocument>, AccountingDocumentAggregator>();

        services.AddHostedService<DomainEventServiceBusListener>();
        services.AddTransient<RefreshAppointmentWhenContactChange>();
        services.AddTransient<RefreshAppointmentWhenTariffChange>();
        services.AddTransient<ProjectAccountingPeriodWhenAppointmentUpdated>();

        QuestPDF.Settings.License = LicenseType.Community;
    }
}