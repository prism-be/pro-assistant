using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Core;
using Prism.Infrastructure.Authentication;
using Prism.Infrastructure.Providers;
using Prism.ProAssistant.Domain.DayToDay.Contacts;

namespace Prism.ProAssistant.Storage.Tests;

public class QueryServiceTests
{
    [Fact]
    public async Task ListAsync_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<QueryService>>();
        var userOrganization = new UserOrganization();
        var stateProvider = new Mock<IStateProvider>();
        var container = new Mock<IStateContainer<Contact>>();
        container.Setup(x => x.ListAsync()).ReturnsAsync(new List<Contact>());
        stateProvider.Setup(x => x.GetContainerAsync<Contact>()).ReturnsAsync(container.Object);

        // Act
        var queryService = new QueryService(logger.Object, userOrganization, stateProvider.Object);
        var result = await queryService.ListAsync<Contact>();

        // Assert
        result.Should().BeEmpty();
        container.Verify(x => x.ListAsync(), Times.Once);
    }

    [Fact]
    public async Task SingleAsync_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<QueryService>>();
        var userOrganization = new UserOrganization();
        var stateProvider = new Mock<IStateProvider>();
        var container = new Mock<IStateContainer<Contact>>();
        container.Setup(x => x.ReadAsync(It.IsAny<string>())).ReturnsAsync(new Contact
        {
            Id = Identifier.GenerateString()
        });
        stateProvider.Setup(x => x.GetContainerAsync<Contact>()).ReturnsAsync(container.Object);

        // Act
        var queryService = new QueryService(logger.Object, userOrganization, stateProvider.Object);
        var result = await queryService.SingleAsync<Contact>(Identifier.GenerateString());

        // Assert
        result.Should().NotBeNull();
        container.Verify(x => x.ReadAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task SingleOrDefaultAsync_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<QueryService>>();
        var userOrganization = new UserOrganization();
        var stateProvider = new Mock<IStateProvider>();
        var container = new Mock<IStateContainer<Contact>>();
        container.Setup(x => x.ReadAsync(It.IsAny<string>())).ReturnsAsync(new Contact
        {
            Id = Identifier.GenerateString()
        });
        stateProvider.Setup(x => x.GetContainerAsync<Contact>()).ReturnsAsync(container.Object);

        // Act
        var queryService = new QueryService(logger.Object, userOrganization, stateProvider.Object);
        var result = await queryService.SingleOrDefaultAsync<Contact>(Identifier.GenerateString());

        // Assert
        result.Should().NotBeNull();
        container.Verify(x => x.ReadAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<QueryService>>();
        var userOrganization = new UserOrganization();
        var stateProvider = new Mock<IStateProvider>();
        var container = new Mock<IStateContainer<Contact>>();
        container.Setup(x => x.SearchAsync()).ReturnsAsync(new List<Contact>());

        // Act
        var queryService = new QueryService(logger.Object, userOrganization, stateProvider.Object);
        var result = await queryService.SearchAsync<Contact>();

        // Assert
        result.Should().BeEmpty();
        container.Verify(x => x.SearchAsync(), Times.Once);
    }
}