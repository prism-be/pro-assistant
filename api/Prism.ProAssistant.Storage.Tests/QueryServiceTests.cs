namespace Prism.ProAssistant.Storage.Tests;

using Core;
using Domain.Accounting.Document;
using Domain.DayToDay.Contacts;
using FluentAssertions;
using Infrastructure.Authentication;
using Infrastructure.Providers;
using Microsoft.Extensions.Logging;
using Moq;

public class QueryServiceTests
{
    [Fact]
    public async Task DistinctAsync_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<QueryService>>();
        var userOrganization = new UserOrganization();
        var stateProvider = new Mock<IStateProvider>();
        var container = new Mock<IStateContainer<Contact>>();
        container.Setup(x => x.Distinct<string>(It.IsAny<string>())).ReturnsAsync(new List<string>());
        stateProvider.Setup(x => x.GetContainerAsync<Contact>()).ReturnsAsync(container.Object);

        // Act
        var queryService = new QueryService(logger.Object, userOrganization, stateProvider.Object);
        var result = await queryService.DistinctAsync<Contact, string>(nameof(Contact.FirstName));

        // Assert
        result.Should().BeEmpty();
        container.Verify(x => x.Distinct<string>(It.IsAny<string>()), Times.Once);
    }

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
    public async Task Max_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<QueryService>>();
        var userOrganization = new UserOrganization();
        var stateProvider = new Mock<IStateProvider>();
        var container = new Mock<IStateContainer<AccountingDocument>>();
        container.Setup(x => x.SearchAsync(It.IsAny<Filter[]>()))
            .ReturnsAsync(new List<AccountingDocument>
            {
                new()
                {
                    Id = Identifier.GenerateString(),
                    DocumentNumber = 1
                },
                new()
                {
                    Id = Identifier.GenerateString(),
                    DocumentNumber = 2
                },
                new()
                {
                    Id = Identifier.GenerateString(),
                    DocumentNumber = 3
                }
            });

        stateProvider.Setup(x => x.GetContainerAsync<AccountingDocument>()).ReturnsAsync(container.Object);

        // Act
        var queryService = new QueryService(logger.Object, userOrganization, stateProvider.Object);
        var result = await queryService.MaxAsync<AccountingDocument, int?>(x => x.DocumentNumber);

        // Assert
        result.Should().Be(3);
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
        stateProvider.Setup(x => x.GetContainerAsync<Contact>()).ReturnsAsync(container.Object);

        // Act
        var queryService = new QueryService(logger.Object, userOrganization, stateProvider.Object);
        var result = await queryService.SearchAsync<Contact>();

        // Assert
        result.Should().BeEmpty();
        container.Verify(x => x.SearchAsync(), Times.Once);
    }

    [Fact]
    public async Task SingleAsync_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<QueryService>>();
        var userOrganization = new UserOrganization();
        var stateProvider = new Mock<IStateProvider>();
        var container = new Mock<IStateContainer<Contact>>();
        container.Setup(x => x.ReadAsync(It.IsAny<string>()))
            .ReturnsAsync(new Contact
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
        container.Setup(x => x.ReadAsync(It.IsAny<string>()))
            .ReturnsAsync(new Contact
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
}