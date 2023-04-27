namespace Prism.ProAssistant.Api.Tests.Services;

using System.Text.Json;
using System.Text.Json.Nodes;
using Api.Services;
using Azure.Messaging.ServiceBus;
using Core;
using Domain;
using Domain.DayToDay.Contacts;
using Domain.DayToDay.Contacts.Events;
using Infrastructure.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Storage;
using Storage.Effects;
using Storage.Events;

public class DomainEventServiceBusListenerTests
{
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
        services.AddSingleton(queryService.Object);
        
        var eventStore = new Mock<IEventStore>();
        services.AddSingleton(eventStore.Object);
        
        var provider = services.BuildServiceProvider();
        
        var serviceBusClient = new Mock<ServiceBusClient>();
        
        var eventContext = new EventContext
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
                        Id = Identifier.GenerateString()
                    }
                }),
                StreamType = "contacts",
                UserId = Identifier.GenerateString()
            },
            Context = new UserOrganization
            {
                Id = Identifier.GenerateString(),
                Organization = Identifier.GenerateString()
            }
        };
        
        // Act
        var listener = new DomainEventServiceBusListener(serviceBusClient.Object, provider);
        await listener.ProcessMessage(eventContext);

        // Assert

    }
}