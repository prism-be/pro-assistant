namespace Prism.ProAssistant.Api.Tests.Services;

using System.Text.Json;
using Api.Services.Listeners;
using Azure.Messaging.ServiceBus;
using Core;
using Domain;
using Domain.DayToDay.Appointments;
using Domain.DayToDay.Contacts;
using Domain.DayToDay.Contacts.Events;
using FluentAssertions;
using Infrastructure.Authentication;
using Infrastructure.Providers;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Storage;
using Storage.Effects;
using Storage.Events;

public class DomainEventServiceBusListenerTests
{
    [Fact]
    public void Listeners_Tests()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton(new UserOrganization
        {
            Id = Identifier.GenerateString(),
            Organization = Identifier.GenerateString()
        });
        
        services.AddTransient<RefreshAppointmentWhenContactChange>();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        
        var serviceBusClient = new Mock<ServiceBusClient>();

        // Act
        var accountingDocumentEventServiceBusListener = new AccountingDocumentEventServiceBusListener(serviceBusClient.Object, serviceProvider);
        var accountingForecastEventServiceBusListener = new AccountingForecastEventServiceBusListener(serviceBusClient.Object, serviceProvider);
        var accountingReportingPeriodEventServiceBusListener = new AccountingReportingPeriodEventServiceBusListener(serviceBusClient.Object, serviceProvider);
        var appointmentEventServiceBusListener = new AppointmentEventServiceBusListener(serviceBusClient.Object, serviceProvider);
        var contactEventServiceBusListener = new ContactEventServiceBusListener(serviceBusClient.Object, serviceProvider);
        var documentConfigurationEventServiceBusListener = new DocumentConfigurationEventServiceBusListener(serviceBusClient.Object, serviceProvider);
        var settingsEventServiceBusListener = new SettingsEventServiceBusListener(serviceBusClient.Object, serviceProvider);
        var tariffEventServiceBusListener = new TariffEventServiceBusListener(serviceBusClient.Object, serviceProvider);

        // Assert
        accountingDocumentEventServiceBusListener.Should().NotBeNull();
        accountingForecastEventServiceBusListener.Should().NotBeNull();
        accountingReportingPeriodEventServiceBusListener.Should().NotBeNull();
        appointmentEventServiceBusListener.Should().NotBeNull();
        contactEventServiceBusListener.Should().NotBeNull();
        documentConfigurationEventServiceBusListener.Should().NotBeNull();
        settingsEventServiceBusListener.Should().NotBeNull();
        tariffEventServiceBusListener.Should().NotBeNull();
    }
    
    [Fact]
    public async Task Execute_Ok()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton(new UserOrganization
        {
            Id = Identifier.GenerateString(),
            Organization = Identifier.GenerateString()
        });
        services.AddTransient<RefreshAppointmentWhenContactChange>();
        services.AddLogging();

        var queryService = new Mock<IQueryService>();

        queryService.Setup(x => x.DistinctAsync<Appointment, string>(It.IsAny<string>(), It.IsAny<Filter[]>()))
            .ReturnsAsync(new List<string>(
                new[]
                {
                    Identifier.GenerateString()
                }
            ));

        services.AddSingleton(queryService.Object);

        var eventStore = new Mock<IEventStore>();
        services.AddSingleton(eventStore.Object);

        var contactId = Identifier.GenerateString();

        var provider = services.BuildServiceProvider();

        var serviceBusClient = new Mock<ServiceBusClient>();

        var eventContext = new EventContext<Contact>
        {
            Event = new DomainEvent
            {
                Id = Identifier.GenerateString(),
                Type = "ContactUpdated",
                StreamId = Identifier.GenerateString(),
                Data = JsonSerializer.Serialize(new ContactUpdated
                {
                    Contact = new Contact
                    {
                        Id = contactId
                    }
                }),
                StreamType = "contacts",
                UserId = Identifier.GenerateString()
            },
            Context = new UserOrganization
            {
                Id = Identifier.GenerateString(),
                Organization = Identifier.GenerateString()
            },
            PreviousState = new Contact
            {
                Id = contactId
            },
            CurrentState = new Contact
            {
                Id = contactId
            }
        };

        // Act
        var listener = new ContactEventServiceBusListener(serviceBusClient.Object, provider);
        await listener.ProcessMessage(eventContext);

        // Assert
        eventStore.Verify(x => x.Persist<Appointment>(It.IsAny<string>()), Times.Once);
    }
}