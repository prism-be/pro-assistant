using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Moq;
using Prism.ProAssistant.Api.Exceptions;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;

namespace Prism.ProAssistant.Api.Tests.Services;

public class QueryServiceTests
{
    

    [Fact]
    public async Task ListAsync_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<QueryService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Contact>>();
        collection.SetupCollection(new Contact
            {
                Id = Identifier.GenerateString()
            },
            new Contact
            {
                Id = Identifier.GenerateString()
            });

        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(collection.Object);

        // Act
        var service = new QueryService(userOrganizationService.Object, logger.Object);
        var result = await service.ListAsync<Contact>();

        // Assert
        result.Count.Should().Be(2);

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeastOnce());
    }

    [Fact]
    public async Task SearchAsync_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var contact = new Contact
        {
            Id = id
        };

        var logger = new Mock<ILogger<QueryService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Contact>>();
        collection.SetupCollection(contact);
        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(collection.Object);

        var filters = new List<SearchFilter>
        {
            new()
                { Field = "field1", Operator = "eq", Value = "value1" },
            new()
                { Field = "field2", Operator = "ne", Value = "value2" },
            new()
                { Field = "field3", Operator = "gt", Value = "value3" },
            new()
                { Field = "field4", Operator = "gte", Value = "value4" },
            new()
                { Field = "field5", Operator = "lt", Value = "value5" },
            new()
                { Field = "field6", Operator = "lte", Value = DateTime.Now },
            new()
                { Field = "field7", Operator = "regex", Value = "(.*)" }
        };

        // Act
        var service = new QueryService(userOrganizationService.Object, logger.Object);
        var result = await service.SearchAsync<Contact>(filters);

        // Assert
        collection.Verify(x => x.FindAsync(It.IsAny<FilterDefinition<Contact>>(), It.IsAny<FindOptions<Contact, Contact>>(), CancellationToken.None), Times.Once);

        result.Count.Should().Be(1);

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeastOnce());
    }

    [Fact]
    public async Task SearchAsync_Unsupported()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var contact = new Contact
        {
            Id = id
        };

        var logger = new Mock<ILogger<QueryService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Contact>>();
        collection.SetupCollection(contact);
        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(collection.Object);

        var filters = new List<SearchFilter>
        {
            new()
                { Field = "field1", Operator = "yolo", Value = "value1" }
        };

        // Act
        var service = new QueryService(userOrganizationService.Object, logger.Object);
        await service.Invoking(s => s.SearchAsync<Contact>(filters))
            .Should().ThrowAsync<NotSupportedException>();
    }

    [Fact]
    public async Task SingleAsync_NotFound()
    {
        // Arrange
        var id = Identifier.GenerateString();

        var logger = new Mock<ILogger<QueryService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Contact>>();
        collection.SetupCollection();

        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(collection.Object);

        // Act
        var service = new QueryService(userOrganizationService.Object, logger.Object);

        await service.Invoking(s => s.SingleAsync<Contact>(id))
            .Should().ThrowAsync<NotFoundException>();

        // Assert
        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeastOnce());
    }

    [Fact]
    public async Task SingleAsync_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();

        var logger = new Mock<ILogger<QueryService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Contact>>();
        collection.SetupCollection(new Contact
        {
            Id = id
        });

        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(collection.Object);

        // Act
        var service = new QueryService(userOrganizationService.Object, logger.Object);
        var result = await service.SingleAsync<Contact>(id);

        // Assert
        result.Id.Should().Be(id);

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeastOnce());
    }

    [Fact]
    public async Task SingleOrDefaultAsync_Null()
    {
        // Arrange
        var id = Identifier.GenerateString();

        var logger = new Mock<ILogger<QueryService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Contact>>();
        collection.SetupCollection();

        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(collection.Object);

        // Act
        var service = new QueryService(userOrganizationService.Object, logger.Object);
        var result = await service.SingleOrDefaultAsync<Contact>(id);

        // Assert
        result.Should().BeNull();

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeastOnce());
    }

    [Fact]
    public async Task SingleOrDefaultAsync_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();

        var logger = new Mock<ILogger<QueryService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Contact>>();
        collection.SetupCollection(new Contact
        {
            Id = id
        });

        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(collection.Object);

        // Act
        var service = new QueryService(userOrganizationService.Object, logger.Object);
        var result = await service.SingleOrDefaultAsync<Contact>(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeastOnce());
    }

    [Fact]
    public async Task SingleOrDefaultAsync_SpecialCase()
    {
        // Arrange
        var id = "000000000000000000000000";

        var logger = new Mock<ILogger<QueryService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Contact>>();
        collection.SetupCollection(new Contact
        {
            Id = id
        });

        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(collection.Object);

        // Act
        var service = new QueryService(userOrganizationService.Object, logger.Object);
        var result = await service.SingleOrDefaultAsync<Contact>(id);

        // Assert
        result.Should().BeNull();

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeastOnce());
    }
}