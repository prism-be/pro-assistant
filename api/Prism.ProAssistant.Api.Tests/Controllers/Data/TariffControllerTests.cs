using Moq;
using Prism.Core;
using Prism.Infrastructure.Providers;
using Prism.ProAssistant.Api.Controllers.Data;
using Prism.ProAssistant.Domain.Configuration.Tariffs;
using Prism.ProAssistant.Domain.Configuration.Tariffs.Events;
using Prism.ProAssistant.Storage;
using Prism.ProAssistant.Storage.Events;

namespace Prism.ProAssistant.Api.Tests.Controllers.Data;

public class TariffControllerTests
{
    [Fact]
    public async Task Insert_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        var tariff = new Tariff
        {
            Id = Identifier.GenerateString(),
            Name = Identifier.GenerateString()
        };

        // Act
        var controller = new TariffController(queryService.Object, eventStore.Object);
        await controller.Insert(tariff);

        // Assert
        eventStore.Verify(x => x.RaiseAndPersist<Tariff>(It.Is<TariffCreated>(y => y.Tariff == tariff)), Times.Once);
    }

    [Fact]
    public async Task List_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        // Act
        var controller = new TariffController(queryService.Object, eventStore.Object);
        await controller.List();

        // Assert
        queryService.Verify(x => x.ListAsync<Tariff>(), Times.Once);
    }

    [Fact]
    public async Task Search_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        // Act
        var controller = new TariffController(queryService.Object, eventStore.Object);
        await controller.Search(Array.Empty<Filter>());

        // Assert
        queryService.Verify(x => x.SearchAsync<Tariff>(Array.Empty<Filter>()), Times.Once);
    }

    [Fact]
    public async Task Single_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        var id = Identifier.GenerateString();

        // Act
        var controller = new TariffController(queryService.Object, eventStore.Object);
        await controller.Single(id);

        // Assert
        queryService.Verify(x => x.SingleOrDefaultAsync<Tariff>(id), Times.Once);
    }

    [Fact]
    public async Task Update_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        var tariff = new Tariff
        {
            Id = Identifier.GenerateString(),
            Name = Identifier.GenerateString()
        };

        // Act
        var controller = new TariffController(queryService.Object, eventStore.Object);
        await controller.Update(tariff);

        // Assert
        eventStore.Verify(x => x.RaiseAndPersist<Tariff>(It.Is<TariffUpdated>(y => y.Tariff == tariff)), Times.Once);
    }
}