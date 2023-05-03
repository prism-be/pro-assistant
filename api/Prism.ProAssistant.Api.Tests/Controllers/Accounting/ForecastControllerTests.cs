namespace Prism.ProAssistant.Api.Tests.Controllers.Accounting;

using Api.Controllers.Data.Accounting;
using Core;
using Domain.Accounting.Forecast;
using Domain.Accounting.Forecast.Events;
using Moq;
using Storage;
using Storage.Events;

public class ForecastControllerTests
{
    [Fact]
    public async Task Delete_Ok()
    {
        // Arrange
        var eventStore = new Mock<IEventStore>();
        var queryService = new Mock<IQueryService>();

        var streamId = Identifier.GenerateString();

        // Act
        var controller = new ForecastController(eventStore.Object, queryService.Object);
        await controller.Delete(new ForecastController.ForecastInformation(streamId, Identifier.GenerateString()));

        // Assert
        eventStore.Verify(x => x.RaiseAndPersist<Forecast>(It.Is<ForecastDeleted>(i => i.StreamId == streamId)), Times.Once);
    }

    [Fact]
    public async Task Insert_Ok()
    {
        // Arrange
        var eventStore = new Mock<IEventStore>();
        var queryService = new Mock<IQueryService>();

        // Act
        var controller = new ForecastController(eventStore.Object, queryService.Object);
        await controller.Insert(new ForecastController.ForecastInformation(Identifier.GenerateString(), Identifier.GenerateString()));

        // Assert
        eventStore.Verify(x => x.RaiseAndPersist<Forecast>(It.IsAny<ForecastCreated>()), Times.Once);
    }

    [Fact]
    public async Task List_Ok()
    {
        // Arrange
        var eventStore = new Mock<IEventStore>();
        var queryService = new Mock<IQueryService>();

        // Act
        var controller = new ForecastController(eventStore.Object, queryService.Object);
        await controller.List();

        // Assert
        queryService.Verify(x => x.ListAsync<Forecast>(), Times.Once);
    }

    [Fact]
    public async Task Update_Ok()
    {
        // Arrange
        var eventStore = new Mock<IEventStore>();
        var queryService = new Mock<IQueryService>();

        var streamId = Identifier.GenerateString();

        // Act
        var controller = new ForecastController(eventStore.Object, queryService.Object);
        await controller.Update(new ForecastController.ForecastInformation(streamId, Identifier.GenerateString()));

        // Assert
        eventStore.Verify(x => x.RaiseAndPersist<Forecast>(It.Is<ForecastUpdated>(i => i.StreamId == streamId)), Times.Once);
    }
}