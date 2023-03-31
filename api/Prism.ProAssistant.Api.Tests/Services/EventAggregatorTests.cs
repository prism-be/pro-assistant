using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;

namespace Prism.ProAssistant.Api.Tests.Services;

public class EventAggregatorTests
{

    [Fact]
    public async Task Aggregate_Aggregate()
    {
        // Arrange
        var contactId = Identifier.GenerateString();

        var eventsCollection = new Mock<IMongoCollection<Event<Contact>>>();
        eventsCollection.SetupCollection(
            new Event<Contact>
            {
                EventType = EventType.Insert, ObjectId = contactId, Data = new Contact
                {
                    Id = contactId
                }
            },
            new Event<Contact>
            {
                EventType = EventType.Update, ObjectId = contactId, Updates = new[]
                {
                    new KeyValuePair<string, string>("FirstName", JsonSerializer.Serialize("John"))
                }
            },
            new Event<Contact>
            {
                EventType = EventType.Update, ObjectId = contactId, Updates = new[]
                {
                    new KeyValuePair<string, string>("LastName", JsonSerializer.Serialize("Doe"))
                }
            },
            new Event<Contact>
            {
                EventType = EventType.Aggregate, ObjectId = contactId, Data = new Contact
                {
                    Id = contactId, FirstName = "Jane", LastName = "Doit"
                }
            }
        );

        var contactCollection = new Mock<IMongoCollection<Contact>>();
        contactCollection.SetupCollection();

        var userOrganizationService = new Mock<IUserOrganizationService>();
        userOrganizationService.Setup(x => x.GetUserEventCollection<Contact>()).ReturnsAsync(eventsCollection.Object);
        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(contactCollection.Object);

        // Act
        var eventAggregator = new EventAggregator(userOrganizationService.Object, Mock.Of<ILogger<EventAggregator>>());
        await eventAggregator.AggregateAsync<Contact>(contactId);

        // Assert
        contactCollection.Verify(x => x.ReplaceOneAsync(It.IsAny<FilterDefinition<Contact>>(), It.Is<Contact>(
            c => c.Id == contactId && c.FirstName == "Jane" && c.LastName == "Doit"
        ), It.IsAny<ReplaceOptions>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Aggregate_Delete()
    {
        // Arrange
        var eventsCollection = new Mock<IMongoCollection<Event<Contact>>>();
        eventsCollection.SetupCollection();

        var contactCollection = new Mock<IMongoCollection<Contact>>();
        contactCollection.SetupCollection();

        var userOrganizationService = new Mock<IUserOrganizationService>();
        userOrganizationService.Setup(x => x.GetUserEventCollection<Contact>()).ReturnsAsync(eventsCollection.Object);
        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(contactCollection.Object);

        var contactId = Identifier.GenerateString();

        // Act
        var eventAggregator = new EventAggregator(userOrganizationService.Object, Mock.Of<ILogger<EventAggregator>>());
        await eventAggregator.AggregateAsync<Contact>(contactId);

        // Assert
        contactCollection.Verify(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<Contact>>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Aggregate_Ok()
    {
        // Arrange
        var contactId = Identifier.GenerateString();

        var eventsCollection = new Mock<IMongoCollection<Event<Contact>>>();
        eventsCollection.SetupCollection(
            new Event<Contact>
            {
                EventType = EventType.Insert, ObjectId = contactId, Data = new Contact
                {
                    Id = contactId
                }
            },
            new Event<Contact>
            {
                EventType = EventType.Update, ObjectId = contactId, Updates = new[]
                {
                    new KeyValuePair<string, string>("FirstName", JsonSerializer.Serialize("John"))
                }
            },
            new Event<Contact>
            {
                EventType = EventType.Update, ObjectId = contactId, Updates = new[]
                {
                    new KeyValuePair<string, string>("LastName", JsonSerializer.Serialize("Doe"))
                }
            }
        );

        var contactCollection = new Mock<IMongoCollection<Contact>>();
        contactCollection.SetupCollection();

        var userOrganizationService = new Mock<IUserOrganizationService>();
        userOrganizationService.Setup(x => x.GetUserEventCollection<Contact>()).ReturnsAsync(eventsCollection.Object);
        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(contactCollection.Object);

        // Act
        var eventAggregator = new EventAggregator(userOrganizationService.Object, Mock.Of<ILogger<EventAggregator>>());
        await eventAggregator.AggregateAsync<Contact>(contactId);

        // Assert
        contactCollection.Verify(x => x.ReplaceOneAsync(It.IsAny<FilterDefinition<Contact>>(), It.Is<Contact>(
            c => c.Id == contactId && c.FirstName == "John" && c.LastName == "Doe"
        ), It.IsAny<ReplaceOptions>(), CancellationToken.None), Times.Once);
    }
    
    [Fact]
    public async Task Aggregate_OkAndDelete()
    {
        // Arrange
        var contactId = Identifier.GenerateString();

        var eventsCollection = new Mock<IMongoCollection<Event<Contact>>>();
        eventsCollection.SetupCollection(
            new Event<Contact>
            {
                EventType = EventType.Insert, ObjectId = contactId, Data = new Contact
                {
                    Id = contactId
                }
            },
            new Event<Contact>
            {
                EventType = EventType.Update, ObjectId = contactId, Updates = new[]
                {
                    new KeyValuePair<string, string>("FirstName", JsonSerializer.Serialize("John"))
                }
            },
            new Event<Contact>
            {
                EventType = EventType.Update, ObjectId = contactId, Updates = new[]
                {
                    new KeyValuePair<string, string>("LastName", JsonSerializer.Serialize("Doe"))
                }
            },
            new Event<Contact>
            {
                EventType = EventType.Delete, ObjectId = contactId
            }
        );

        var contactCollection = new Mock<IMongoCollection<Contact>>();
        contactCollection.SetupCollection();

        var userOrganizationService = new Mock<IUserOrganizationService>();
        userOrganizationService.Setup(x => x.GetUserEventCollection<Contact>()).ReturnsAsync(eventsCollection.Object);
        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(contactCollection.Object);

        // Act
        var eventAggregator = new EventAggregator(userOrganizationService.Object, Mock.Of<ILogger<EventAggregator>>());
        await eventAggregator.AggregateAsync<Contact>(contactId);

        // Assert
        contactCollection.Verify(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<Contact>>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Aggregate_Replace()
    {
        // Arrange
        var contactId = Identifier.GenerateString();

        var eventsCollection = new Mock<IMongoCollection<Event<Contact>>>();
        eventsCollection.SetupCollection(
            new Event<Contact>
            {
                EventType = EventType.Insert, ObjectId = contactId, Data = new Contact
                {
                    Id = contactId
                }
            },
            new Event<Contact>
            {
                EventType = EventType.Update, ObjectId = contactId, Updates = new[]
                {
                    new KeyValuePair<string, string>("FirstName", JsonSerializer.Serialize("John"))
                }
            },
            new Event<Contact>
            {
                EventType = EventType.Update, ObjectId = contactId, Updates = new[]
                {
                    new KeyValuePair<string, string>("LastName", JsonSerializer.Serialize("Doe"))
                }
            },
            new Event<Contact>
            {
                EventType = EventType.Replace, ObjectId = contactId, Data = new Contact
                {
                    Id = contactId, FirstName = "Jane", LastName = "Doit"
                }
            }
        );

        var contactCollection = new Mock<IMongoCollection<Contact>>();
        contactCollection.SetupCollection();

        var userOrganizationService = new Mock<IUserOrganizationService>();
        userOrganizationService.Setup(x => x.GetUserEventCollection<Contact>()).ReturnsAsync(eventsCollection.Object);
        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(contactCollection.Object);

        // Act
        var eventAggregator = new EventAggregator(userOrganizationService.Object, Mock.Of<ILogger<EventAggregator>>());
        await eventAggregator.AggregateAsync<Contact>(contactId);

        // Assert
        contactCollection.Verify(x => x.ReplaceOneAsync(It.IsAny<FilterDefinition<Contact>>(), It.Is<Contact>(
            c => c.Id == contactId && c.FirstName == "Jane" && c.LastName == "Doit"
        ), It.IsAny<ReplaceOptions>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Aggregate_UpdateNull()
    {
        // Arrange
        var contactId = Identifier.GenerateString();

        var eventsCollection = new Mock<IMongoCollection<Event<Contact>>>();
        eventsCollection.SetupCollection(
            new Event<Contact>
            {
                EventType = EventType.Update, ObjectId = contactId, Updates = new[]
                {
                    new KeyValuePair<string, string>("FirstName", JsonSerializer.Serialize("John"))
                }
            }
        );

        var contactCollection = new Mock<IMongoCollection<Contact>>();
        contactCollection.SetupCollection();

        var userOrganizationService = new Mock<IUserOrganizationService>();
        userOrganizationService.Setup(x => x.GetUserEventCollection<Contact>()).ReturnsAsync(eventsCollection.Object);
        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(contactCollection.Object);

        // Act
        var eventAggregator = new EventAggregator(userOrganizationService.Object, Mock.Of<ILogger<EventAggregator>>());
        var act = async () => await eventAggregator.AggregateAsync<Contact>(contactId);
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
    
    [Fact]
    public async Task Aggregate_UpdatePrepertyNotExist()
    {
        // Arrange
        var contactId = Identifier.GenerateString();

        var eventsCollection = new Mock<IMongoCollection<Event<Contact>>>();
        eventsCollection.SetupCollection(
            new Event<Contact>
            {
                EventType = EventType.Insert, ObjectId = contactId, Data = new Contact
                {
                    Id = contactId
                }
            },
            new Event<Contact>
            {
                EventType = EventType.Update, ObjectId = contactId, Updates = new[]
                {
                    new KeyValuePair<string, string>("FirstNameOf", JsonSerializer.Serialize("John"))
                }
            }
        );

        var contactCollection = new Mock<IMongoCollection<Contact>>();
        contactCollection.SetupCollection();

        var userOrganizationService = new Mock<IUserOrganizationService>();
        userOrganizationService.Setup(x => x.GetUserEventCollection<Contact>()).ReturnsAsync(eventsCollection.Object);
        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(contactCollection.Object);

        // Act
        var eventAggregator = new EventAggregator(userOrganizationService.Object, Mock.Of<ILogger<EventAggregator>>());
        var act = async () => await eventAggregator.AggregateAsync<Contact>(contactId);
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Aggregate_UpdatesNull()
    {
        // Arrange
        var contactId = Identifier.GenerateString();

        var eventsCollection = new Mock<IMongoCollection<Event<Contact>>>();
        eventsCollection.SetupCollection(
            new Event<Contact>
            {
                EventType = EventType.Insert, ObjectId = contactId, Data =
                    new Contact
                    {
                        Id = contactId
                    }
            },
            new Event<Contact>
            {
                EventType = EventType.Update, ObjectId = contactId
            }
        );

        var contactCollection = new Mock<IMongoCollection<Contact>>();
        contactCollection.SetupCollection();

        var userOrganizationService = new Mock<IUserOrganizationService>();
        userOrganizationService.Setup(x => x.GetUserEventCollection<Contact>()).ReturnsAsync(eventsCollection.Object);
        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(contactCollection.Object);

        // Act
        var eventAggregator = new EventAggregator(userOrganizationService.Object, Mock.Of<ILogger<EventAggregator>>());
        var act = async () => await eventAggregator.AggregateAsync<Contact>(contactId);
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}