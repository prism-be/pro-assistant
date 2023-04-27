namespace Prism.ProAssistant.Api.Tests.Controllers;

using Api.Controllers;
using Domain;
using Domain.DayToDay.Contacts;
using Infrastructure.Providers;
using Microsoft.Extensions.Logging;
using Moq;
using Storage;
using Storage.Events;

public class MaintenanceControllerTests
{
    [Fact]
    public async Task RebuildProjections_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<MaintenanceController>>();
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();
        var streams = new List<string> { "stream1", "stream2" };
        queryService.Setup(x => x.DistinctAsync<DomainEvent, string>(nameof(DomainEvent.StreamId), It.IsAny<Filter[]>())).ReturnsAsync(streams);
        
        // Act
        var maintenanceController = new MaintenanceController(logger.Object, queryService.Object, eventStore.Object);
        await maintenanceController.RebuildProjections();

        // Assert
        eventStore.Verify(x => x.Persist<Contact>("stream1"), Times.Once);
        eventStore.Verify(x => x.Persist<Contact>("stream2"), Times.Once);
    }
}