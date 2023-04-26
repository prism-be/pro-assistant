using Moq;
using Prism.Core;
using Prism.Infrastructure.Providers;
using Prism.ProAssistant.Api.Controllers.Data;
using Prism.ProAssistant.Domain.Configuration.Settings;
using Prism.ProAssistant.Domain.Configuration.Settings.Events;
using Prism.ProAssistant.Storage;
using Prism.ProAssistant.Storage.Events;

namespace Prism.ProAssistant.Api.Tests.Controllers.Data;

using Domain;

public class SettingControllerTests
{
    [Fact]
    public async Task Insert_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        var setting = new Setting
        {
            Id = Identifier.GenerateString()
        };

        // Act
        var controller = new SettingController(queryService.Object, eventStore.Object);
        await controller.Insert(setting);

        // Assert
        eventStore.Verify(x => x.RaiseAndPersist<Setting>(It.Is<SettingCreated>(y => y.Setting == setting)), Times.Once);
    }

    [Fact]
    public async Task List_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        // Act
        var controller = new SettingController(queryService.Object, eventStore.Object);
        await controller.List();

        // Assert
        queryService.Verify(x => x.ListAsync<Setting>(), Times.Once);
    }

    [Fact]
    public async Task Search_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        // Act
        var controller = new SettingController(queryService.Object, eventStore.Object);
        await controller.Search(Array.Empty<Filter>());

        // Assert
        queryService.Verify(x => x.SearchAsync<Setting>(Array.Empty<Filter>()), Times.Once);
    }

    [Fact]
    public async Task Single_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        var id = Identifier.GenerateString();

        // Act
        var controller = new SettingController(queryService.Object, eventStore.Object);
        await controller.Single(id);

        // Assert
        queryService.Verify(x => x.SingleOrDefaultAsync<Setting>(id), Times.Once);
    }

    [Fact]
    public async Task Update_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        var setting = new Setting
        {
            Id = Identifier.GenerateString()
        };

        // Act
        var controller = new SettingController(queryService.Object, eventStore.Object);
        await controller.Update(setting);

        // Assert
        eventStore.Verify(x => x.RaiseAndPersist<Setting>(It.Is<SettingUpdated>(y => y.Setting == setting)), Times.Once);
    }

    [Fact]
    public async Task UpdateMany_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        var settings = new List<Setting>
        {
            new()
            {
                Id = Identifier.GenerateString()
            },
            new()
            {
                Id = Identifier.GenerateString()
            }
        };

        // Act
        var controller = new SettingController(queryService.Object, eventStore.Object);
        await controller.UpdateMany(settings);

        // Assert
        eventStore.Verify(x => x.RaiseAndPersist<Setting>(It.Is<SettingUpdated>(y => y.Setting == settings[0])), Times.Once);
        eventStore.Verify(x => x.RaiseAndPersist<Setting>(It.Is<SettingUpdated>(y => y.Setting == settings[1])), Times.Once);
    }
}