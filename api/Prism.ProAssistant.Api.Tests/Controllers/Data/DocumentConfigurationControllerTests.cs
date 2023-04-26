using Moq;
using Prism.Core;
using Prism.Infrastructure.Providers;
using Prism.ProAssistant.Api.Controllers.Data;
using Prism.ProAssistant.Domain.Configuration.DocumentConfiguration;
using Prism.ProAssistant.Domain.Configuration.DocumentConfiguration.Events;
using Prism.ProAssistant.Storage;
using Prism.ProAssistant.Storage.Events;

namespace Prism.ProAssistant.Api.Tests.Controllers.Data;

public class DocumentConfigurationControllerTests
{
    [Fact]
    public async Task Delete_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        var id = Identifier.GenerateString();

        // Act
        var controller = new DocumentConfigurationController(queryService.Object, eventStore.Object);
        await controller.Delete(id);

        // Assert
        eventStore.Verify(x => x.RaiseAndPersist<DocumentConfiguration>(It.Is<DocumentConfigurationDeleted>(y => y.StreamId == id)), Times.Once);
    }

    [Fact]
    public async Task Insert_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        var cfg = new DocumentConfiguration
        {
            Id = Identifier.GenerateString()
        };

        // Act
        var controller = new DocumentConfigurationController(queryService.Object, eventStore.Object);
        await controller.Insert(cfg);

        // Assert
        eventStore.Verify(x => x.RaiseAndPersist<DocumentConfiguration>(It.Is<DocumentConfigurationCreated>(y => y.DocumentConfiguration == cfg)), Times.Once);
    }

    [Fact]
    public async Task List_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        // Act
        var controller = new DocumentConfigurationController(queryService.Object, eventStore.Object);
        await controller.List();

        // Assert
        queryService.Verify(x => x.ListAsync<DocumentConfiguration>(), Times.Once);
    }

    [Fact]
    public async Task Search_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        // Act
        var controller = new DocumentConfigurationController(queryService.Object, eventStore.Object);
        await controller.Search(Array.Empty<Filter>());

        // Assert
        queryService.Verify(x => x.SearchAsync<DocumentConfiguration>(It.IsAny<Filter[]>()), Times.Once);
    }

    [Fact]
    public async Task Single_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        var id = Identifier.GenerateString();

        // Act
        var controller = new DocumentConfigurationController(queryService.Object, eventStore.Object);
        await controller.Single(id);

        // Assert
        queryService.Verify(x => x.SingleOrDefaultAsync<DocumentConfiguration>(id), Times.Once);
    }

    [Fact]
    public async Task Update_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();
        
        var cfg = new DocumentConfiguration
        {
            Id = Identifier.GenerateString()
        };
        
        // Act
        var controller = new DocumentConfigurationController(queryService.Object, eventStore.Object);
        await controller.Update(cfg);
        
        // Assert
        eventStore.Verify(x => x.RaiseAndPersist<DocumentConfiguration>(It.Is<DocumentConfigurationUpdated>(y => y.DocumentConfiguration == cfg)), Times.Once);
    }
}