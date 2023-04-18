using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Core;
using Prism.Infrastructure.Authentication;
using Prism.Infrastructure.Providers;
using Prism.ProAssistant.Domain;
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
        var container = new Mock<IStateContainer<Dummy>>();
        var stateProvider = new Mock<IStateProvider>();
        stateProvider.Setup(x => x.GetContainerAsync<Dummy>()).ReturnsAsync(container.Object);
        stateProvider.Setup(x => x.GetContainerAsync<DomainEvent>()).ReturnsAsync(eventContainer.Object);

        // Act
        var eventStore = new EventStore(logger.Object, stateProvider.Object, userOrganization);
        await eventStore.Raise(new DummyDomainEvent());

        // Assert
        eventContainer.Verify(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<DomainEvent>()), Times.Once);
        container.Verify(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<Dummy>()), Times.Never);
    }

    [Fact]
    public async Task RaiseAndPersist_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<EventStore>>();
        var userOrganization = new UserOrganization();

        var eventContainer = new Mock<IStateContainer<DomainEvent>>();
        var container = new Mock<IStateContainer<Dummy>>();
        var stateProvider = new Mock<IStateProvider>();
        eventContainer.Setup(x => x.FetchAsync(It.IsAny<Filter[]>())).ReturnsAsync(new List<DomainEvent>
        {
            new()
            {
                Data = JsonSerializer.Serialize(new DummyDomainEvent()),
                Type = nameof(DummyDomainEvent),
                Id = Identifier.GenerateString(),
                StreamId = Identifier.GenerateString(),
                UserId = Identifier.GenerateString()
            }
        });
        stateProvider.Setup(x => x.GetContainerAsync<Dummy>()).ReturnsAsync(container.Object);
        stateProvider.Setup(x => x.GetContainerAsync<DomainEvent>()).ReturnsAsync(eventContainer.Object);

        // Act
        var eventStore = new EventStore(logger.Object, stateProvider.Object, userOrganization);
        await eventStore.RaiseAndPersist<DummyDomainEventAggregator, Dummy>(new DummyDomainEvent());

        // Assert
        eventContainer.Verify(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<DomainEvent>()), Times.Once);
        container.Verify(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<Dummy>()), Times.Once);
    }

    public class Dummy
    {
        public int Value { get; set; }
    }

    private class DummyDomainEvent : IDomainEvent
    {

        public string StreamId => "42";
    }

    private class DummyDomainEventAggregator : IDomainAggregator<Dummy>
    {

        public void Init(string id)
        {
            State = new Dummy { Value = 21 };
        }

        public Dummy State { get; private set; } = new();

        public void When(DomainEvent @event)
        {
            State.Value += 1;
        }
    }
}