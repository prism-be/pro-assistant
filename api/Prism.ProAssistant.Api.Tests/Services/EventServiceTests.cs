using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;

namespace Prism.ProAssistant.Api.Tests.Services;

public class EventServiceTests
{
    [Fact]
    public async Task AggregateAsync_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var contact = new Contact
        {
            Id = id
        };

        var logger = new Mock<ILogger<EventService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var eventCollection = new Mock<IMongoCollection<Event<Contact>>>();
        userOrganizationService.Setup(x => x.GetUserEventCollection<Contact>()).ReturnsAsync(eventCollection.Object);
        
        var dataCollection = new Mock<IMongoCollection<Contact>>();
        dataCollection.SetupCollection(contact);
        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(dataCollection.Object);
        
        var aggregator = new Mock<IEventAggregator>();
        
        // Act
        var service = new EventService(userOrganizationService.Object, logger.Object, aggregator.Object);
        await service.AggregateAsync<Contact>(contact);
        
        // Assert
        eventCollection.Verify(x => x.DeleteManyAsync(It.IsAny<FilterDefinition<Event<Contact>>>(), CancellationToken.None), Times.Once);
        eventCollection.Verify(x => x.InsertOneAsync(It.Is<Event<Contact>>(c => c.Data == contact && c.EventType == EventType.Aggregate), null, CancellationToken.None), Times.Once);
        aggregator.Verify(x => x.AggregateAsync<Contact>(id), Times.Once);
    }
    
    [Fact]
    public async Task UpdateManyAsync_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var contact = new Contact
        {
            Id = id
        };

        var logger = new Mock<ILogger<EventService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var eventCollection = new Mock<IMongoCollection<Event<Contact>>>();
        userOrganizationService.Setup(x => x.GetUserEventCollection<Contact>()).ReturnsAsync(eventCollection.Object);
        
        var dataCollection = new Mock<IMongoCollection<Contact>>();
        dataCollection.SetupCollection(contact);
        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(dataCollection.Object);
        
        var aggregator = new Mock<IEventAggregator>();
        
        // Act
        var service = new EventService(userOrganizationService.Object, logger.Object, aggregator.Object);
        var result = await service.UpdateManyAsync<Contact>(new FieldValue("Id", Identifier.GenerateString()), new FieldValue(nameof(Contact.Id), contact.Id));

        // Assert
        result.Count.Should().Be(1);
        
        eventCollection.Verify(x => x.InsertOneAsync(It.Is<Event<Contact>>(c => c.ObjectId == contact.Id && c.EventType == EventType.Update), null, CancellationToken.None), Times.Once);
        aggregator.Verify(x => x.AggregateAsync<Contact>(id), Times.Once);

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
    public async Task CreateAsync_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var contact = new Contact
        {
            Id = id
        };

        var logger = new Mock<ILogger<EventService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Event<Contact>>>();
        userOrganizationService.Setup(x => x.GetUserEventCollection<Contact>()).ReturnsAsync(collection.Object);

        var aggregator = new Mock<IEventAggregator>();

        // Act
        var service = new EventService(userOrganizationService.Object, logger.Object, aggregator.Object);
        var result = await service.CreateAsync(contact);

        // Assert
        collection.Verify(x => x.InsertOneAsync(It.Is<Event<Contact>>(c => c.Data == contact && c.EventType == EventType.Insert), null, CancellationToken.None), Times.Once);
        aggregator.Verify(x => x.AggregateAsync<Contact>(result.Id!), Times.Once);

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
    public async Task DeleteAsync_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<EventService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Event<Contact>>>();
        userOrganizationService.Setup(x => x.GetUserEventCollection<Contact>()).ReturnsAsync(collection.Object);

        var id = Identifier.GenerateString();

        var aggregator = new Mock<IEventAggregator>();

        // Act
        var service = new EventService(userOrganizationService.Object, logger.Object, aggregator.Object);
        await service.DeleteAsync<Contact>(id);

        // Assert
        collection.Verify(x => x.InsertOneAsync(It.Is<Event<Contact>>(c => c.ObjectId == id && c.EventType == EventType.Delete), null, CancellationToken.None), Times.Once);

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
    public async Task ReplaceAsync_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var contact = new Contact
        {
            Id = id
        };

        var logger = new Mock<ILogger<EventService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Event<Contact>>>();
        userOrganizationService.Setup(x => x.GetUserEventCollection<Contact>()).ReturnsAsync(collection.Object);

        var aggregator = new Mock<IEventAggregator>();

        // Act
        var service = new EventService(userOrganizationService.Object, logger.Object, aggregator.Object);
        await service.ReplaceAsync(contact);

        // Assert
        collection.Verify(x => x.InsertOneAsync(It.Is<Event<Contact>>(c => c.Data == contact && c.EventType == EventType.Replace), null, CancellationToken.None), Times.Once);
        aggregator.Verify(x => x.AggregateAsync<Contact>(id), Times.Once);

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
    public async Task ReplaceManyAsync_ShouldInsert()
    {
        // Arrange
        var id1 = Identifier.GenerateString();
        var contact1 = new Contact
        {
            Id = id1
        };

        var id2 = Identifier.GenerateString();
        var contact2 = new Contact
        {
            Id = id2
        };

        var logger = new Mock<ILogger<EventService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Event<Contact>>>();
        userOrganizationService.Setup(x => x.GetUserEventCollection<Contact>()).ReturnsAsync(collection.Object);

        var aggregator = new Mock<IEventAggregator>();

        // Act
        var service = new EventService(userOrganizationService.Object, logger.Object, aggregator.Object);
        var result = await service.ReplaceManyAsync(new List<Contact>
            { contact1, contact2 });

        // Assert
        collection.Verify(x => x.InsertOneAsync(It.Is<Event<Contact>>(c => c.Data == contact1 && c.EventType == EventType.Replace), null, CancellationToken.None), Times.Once);
        aggregator.Verify(x => x.AggregateAsync<Contact>(contact1.Id), Times.Once);

        collection.Verify(x => x.InsertOneAsync(It.Is<Event<Contact>>(c => c.Data == contact2 && c.EventType == EventType.Replace), null, CancellationToken.None), Times.Once);
        aggregator.Verify(x => x.AggregateAsync<Contact>(contact2.Id), Times.Once);

        result.Count.Should().Be(2);

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeast(2));
    }

    [Fact]
    public async Task UpdateAsync_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var contact = new Contact
        {
            Id = id
        };

        var logger = new Mock<ILogger<EventService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Event<Contact>>>();
        userOrganizationService.Setup(x => x.GetUserEventCollection<Contact>()).ReturnsAsync(collection.Object);

        var aggregator = new Mock<IEventAggregator>();

        // Act
        var service = new EventService(userOrganizationService.Object, logger.Object, aggregator.Object);
        await service.UpdateAsync<Contact>(contact.Id, new FieldValue(nameof(Contact.Id), contact.Id));

        // Assert
        collection.Verify(x => x.InsertOneAsync(It.Is<Event<Contact>>(c => c.ObjectId == contact.Id && c.EventType == EventType.Update), null, CancellationToken.None), Times.Once);
        aggregator.Verify(x => x.AggregateAsync<Contact>(id), Times.Once);

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