namespace Prism.ProAssistant.Api.Tests.Controllers.Data;

using Api.Controllers.Data;
using Core;
using Domain.Configuration.Tariffs;
using Domain.Configuration.Tariffs.Events;
using Domain.DayToDay.Appointments;
using Infrastructure.Providers;
using Moq;
using Storage;
using Storage.Events;

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

    [Fact]
    public async Task Update_Ok_Updated()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        var tariff = new Tariff
        {
            Id = Identifier.GenerateString(),
            Name = Identifier.GenerateString()
        };

        queryService.Setup(x => x.SingleOrDefaultAsync<Tariff>(tariff.Id)).ReturnsAsync(tariff);
        queryService.Setup(x => x.SearchAsync<Appointment>(It.IsAny<Filter[]>()))
            .ReturnsAsync(new List<Appointment>
            {
                new()
                {
                    Id = Identifier.GenerateString(),
                    Title = Identifier.GenerateString(),
                    FirstName = Identifier.GenerateString(),
                    LastName = Identifier.GenerateString()
                },
                new()
                {
                    Id = Identifier.GenerateString(),
                    Title = Identifier.GenerateString(),
                    FirstName = Identifier.GenerateString(),
                    LastName = Identifier.GenerateString()
                }
            });

        // Act
        var controller = new TariffController(queryService.Object, eventStore.Object);
        await controller.Update(tariff);

        // Assert
        eventStore.Verify(x => x.RaiseAndPersist<Tariff>(It.Is<TariffUpdated>(y => y.Tariff == tariff)), Times.Once);
        eventStore.Verify(x => x.Persist<Appointment>(It.IsAny<string>()), Times.Exactly(2));
    }
}