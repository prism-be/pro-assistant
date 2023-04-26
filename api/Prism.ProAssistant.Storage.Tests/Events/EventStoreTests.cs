using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Core;
using Prism.Infrastructure.Authentication;
using Prism.Infrastructure.Providers;
using Prism.ProAssistant.Domain;
using Prism.ProAssistant.Domain.Configuration.DocumentConfiguration;
using Prism.ProAssistant.Domain.Configuration.DocumentConfiguration.Events;
using Prism.ProAssistant.Domain.DayToDay.Contacts;
using Prism.ProAssistant.Domain.DayToDay.Contacts.Events;
using Prism.ProAssistant.Storage.Events;

namespace Prism.ProAssistant.Storage.Tests.Events;

public class EventStoreTests
{

    [Fact]
    public async Task Raise_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<EventStore>>();
        var userOrganization = new UserOrganization();

        var eventContainer = new Mock<IStateContainer<DomainEvent>>();
        var container = new Mock<IStateContainer<Contact>>();
        var stateProvider = new Mock<IStateProvider>();
        var @event = new ContactCreated
        {
            Contact = new Contact
                { Id = Identifier.GenerateString() }
        };
        stateProvider.Setup(x => x.GetContainerAsync<Contact>()).ReturnsAsync(container.Object);
        stateProvider.Setup(x => x.GetContainerAsync<DomainEvent>()).ReturnsAsync(eventContainer.Object);

        // Act
        var eventStore = new EventStore(logger.Object, stateProvider.Object, userOrganization);
        await eventStore.Raise(@event);

        // Assert
        eventContainer.Verify(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<DomainEvent>()), Times.Once);
        container.Verify(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<Contact>()), Times.Never);
    }

    [Fact]
    public async Task RaiseAndPersist_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<EventStore>>();
        var userOrganization = new UserOrganization();

        var eventContainer = new Mock<IStateContainer<DomainEvent>>();
        var container = new Mock<IStateContainer<Contact>>();
        var stateProvider = new Mock<IStateProvider>();
        var @event = new ContactCreated
        {
            Contact = new Contact
                { Id = Identifier.GenerateString() }
        };
        eventContainer.Setup(x => x.FetchAsync(It.IsAny<Filter[]>())).ReturnsAsync(new List<DomainEvent>
        {
            new()
            {
                Data = JsonSerializer.Serialize(@event),
                Type = nameof(ContactCreated),
                Id = Identifier.GenerateString(),
                StreamId = Identifier.GenerateString(),
                StreamType = "contacts",
                UserId = Identifier.GenerateString()
            }
        });
        stateProvider.Setup(x => x.GetContainerAsync<Contact>()).ReturnsAsync(container.Object);
        stateProvider.Setup(x => x.GetContainerAsync<DomainEvent>()).ReturnsAsync(eventContainer.Object);

        // Act
        var eventStore = new EventStore(logger.Object, stateProvider.Object, userOrganization);
        await eventStore.RaiseAndPersist<Contact>(@event);

        // Assert
        eventContainer.Verify(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<DomainEvent>()), Times.Once);
        container.Verify(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<Contact>()), Times.Once);
    }
    
    [Fact]
    public async Task Persist_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<EventStore>>();
        var userOrganization = new UserOrganization();

        var eventContainer = new Mock<IStateContainer<DomainEvent>>();
        var container = new Mock<IStateContainer<Contact>>();
        var stateProvider = new Mock<IStateProvider>();
        var @event = new ContactCreated
        {
            Contact = new Contact
                { Id = Identifier.GenerateString() }
        };
        eventContainer.Setup(x => x.FetchAsync(It.IsAny<Filter[]>())).ReturnsAsync(new List<DomainEvent>
        {
            new()
            {
                Data = JsonSerializer.Serialize(@event),
                Type = nameof(ContactCreated),
                Id = Identifier.GenerateString(),
                StreamId = Identifier.GenerateString(),
                StreamType = "contacts",
                UserId = Identifier.GenerateString()
            }
        });
        stateProvider.Setup(x => x.GetContainerAsync<Contact>()).ReturnsAsync(container.Object);
        stateProvider.Setup(x => x.GetContainerAsync<DomainEvent>()).ReturnsAsync(eventContainer.Object);

        // Act
        var eventStore = new EventStore(logger.Object, stateProvider.Object, userOrganization);
        await eventStore.Persist<Contact>(@event.StreamId);

        // Assert
        eventContainer.Verify(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<DomainEvent>()), Times.Never);
        container.Verify(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<Contact>()), Times.Once);
    }

    [Fact]
    public async Task RaiseAndPersist_Ok_Delete()
    {
        // Arrange
        var logger = new Mock<ILogger<EventStore>>();
        var userOrganization = new UserOrganization();
        var streamId = Identifier.GenerateString();

        var eventContainer = new Mock<IStateContainer<DomainEvent>>();
        var container = new Mock<IStateContainer<DocumentConfiguration>>();
        var stateProvider = new Mock<IStateProvider>();
        var @event = new DocumentConfigurationDeleted
        {
            StreamId = streamId
        };
        eventContainer.Setup(x => x.FetchAsync(It.IsAny<Filter[]>())).ReturnsAsync(new List<DomainEvent>
        {
            new()
            {
                Data = JsonSerializer.Serialize(@event),
                Type = nameof(DocumentConfigurationDeleted),
                Id = Identifier.GenerateString(),
                StreamId = streamId,
                StreamType = "documents-configuration",
                UserId = Identifier.GenerateString()
            }
        });
        stateProvider.Setup(x => x.GetContainerAsync<DocumentConfiguration>()).ReturnsAsync(container.Object);
        stateProvider.Setup(x => x.GetContainerAsync<DomainEvent>()).ReturnsAsync(eventContainer.Object);

        // Act
        var eventStore = new EventStore(logger.Object, stateProvider.Object, userOrganization);
        await eventStore.RaiseAndPersist<DocumentConfiguration>(@event);

        // Assert
        eventContainer.Verify(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<DomainEvent>()), Times.Once);
        container.Verify(x => x.DeleteAsync(It.IsAny<string>()), Times.Once);
    }
}