using Microsoft.Extensions.Logging;
using Moq;
using Prism.ProAssistant.Api.Controllers.Maintenance;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;

namespace Prism.ProAssistant.Api.Tests.Controllers;

public class RebuildEventControllerTests
{
    [Fact]
    public async Task Rebuild_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<RebuildEventController>>();
        var dataService = new Mock<IDataService>();
        var eventService = new Mock<IEventService>();

        dataService.Setup(x => x.ListAsync<Appointment>()).ReturnsAsync(new List<Appointment>
        {
            new()
            {
                Id = Identifier.GenerateString(),
                FirstName = Identifier.GenerateString(),
                LastName = Identifier.GenerateString(),
                Title = Identifier.GenerateString()
            }
        });

        dataService.Setup(x => x.ListAsync<Contact>()).ReturnsAsync(new List<Contact>
        {
            new()
            {
                Id = Identifier.GenerateString(),
                FirstName = Identifier.GenerateString(),
                LastName = Identifier.GenerateString(),
                Title = Identifier.GenerateString()
            }
        });

        dataService.Setup(x => x.ListAsync<DocumentConfiguration>()).ReturnsAsync(new List<DocumentConfiguration>
        {
            new()
            {
                Id = Identifier.GenerateString(),
                Name = Identifier.GenerateString()
            }
        });

        dataService.Setup(x => x.ListAsync<Setting>()).ReturnsAsync(new List<Setting>
        {
            new()
            {
                Id = Identifier.GenerateString(),
                Value = Identifier.GenerateString()
            }
        });

        dataService.Setup(x => x.ListAsync<Tariff>()).ReturnsAsync(new List<Tariff>
        {
            new()
            {
                Id = Identifier.GenerateString(),
                Name = Identifier.GenerateString()
            }
        });

        // Act
        var controller = new RebuildEventController(logger.Object, dataService.Object, eventService.Object);
        await controller.Rebuild();

        // Assert
        eventService.Verify(x => x.AggregateAsync(It.IsAny<Appointment>()), Times.Once);
        eventService.Verify(x => x.AggregateAsync(It.IsAny<Contact>()), Times.Once);
        eventService.Verify(x => x.AggregateAsync(It.IsAny<DocumentConfiguration>()), Times.Once);
        eventService.Verify(x => x.AggregateAsync(It.IsAny<Setting>()), Times.Once);
        eventService.Verify(x => x.AggregateAsync(It.IsAny<Tariff>()), Times.Once);
    }
}