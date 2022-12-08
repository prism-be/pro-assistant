// -----------------------------------------------------------------------
//  <copyright file = "UpsertOneTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Business.Commands;
using Prism.ProAssistant.Business.Events;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Tests.Commands;

public class UpsertOneTests
{
    [Fact]
    public async Task UpsertOneHandler_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var meeting = new Meeting
        {
            Id = id
        };
        
        var logger = new Mock<ILogger<UpsertOneHandler<Meeting>>>();
        var organizationContext = new Mock<IOrganizationContext>();
        var userContextAccessor = new Mock<IUserContextAccessor>();
        var publisher = new Mock<IPublisher>();

        var collection = new Mock<IMongoCollection<Meeting>>();
        organizationContext.Setup(x => x.GetCollection<Meeting>())
            .Returns(collection.Object);

        collection.Setup(x => x.FindOneAndReplaceAsync(It.IsAny<FilterDefinition<Meeting>>(), It.IsAny<Meeting>(), It.IsAny<FindOneAndReplaceOptions<Meeting>>(), CancellationToken.None))
            .ReturnsAsync(meeting);
        collection.SetupCollection();
        
        var collectionHistory = new Mock<IMongoCollection<History>>();
        organizationContext.Setup(x => x.GetCollection<History>())
            .Returns(collectionHistory.Object);

        // Act
        var request = new UpsertOne<Meeting>(meeting);
        var handler = new UpsertOneHandler<Meeting>(logger.Object, organizationContext.Object, userContextAccessor.Object, publisher.Object);
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        collection.Verify(x => x.FindOneAndReplaceAsync(It.IsAny<FilterDefinition<Meeting>>(), meeting, It.IsAny<FindOneAndReplaceOptions<Meeting>>(), CancellationToken.None));
        collectionHistory.Verify(x => x.InsertOneAsync(It.IsAny<History>(), null, CancellationToken.None));
        publisher.Verify(x => x.Publish(It.IsAny<string>(), It.IsAny<UpsertedItem<Meeting>>()));
        result.Id.Should().Be(id);
    }
    
    [Fact]
    public async Task UpsertOneHandler_New()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var meeting = new Meeting();
        
        var logger = new Mock<ILogger<UpsertOneHandler<Meeting>>>();
        var organizationContext = new Mock<IOrganizationContext>();
        var userContextAccessor = new Mock<IUserContextAccessor>();
        var publisher = new Mock<IPublisher>();

        var collection = new Mock<IMongoCollection<Meeting>>();
        organizationContext.Setup(x => x.GetCollection<Meeting>())
            .Returns(collection.Object);
        collection.Setup(x => x.InsertOneAsync(It.IsAny<Meeting>(), null, CancellationToken.None))
            .Callback(() => meeting.Id = id);

        var collectionHistory = new Mock<IMongoCollection<History>>();
        organizationContext.Setup(x => x.GetCollection<History>())
            .Returns(collectionHistory.Object);

        // Act
        var request = new UpsertOne<Meeting>(meeting);
        var handler = new UpsertOneHandler<Meeting>(logger.Object, organizationContext.Object, userContextAccessor.Object, publisher.Object);
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        collection.Verify(x => x.InsertOneAsync(meeting, null, CancellationToken.None));
        collectionHistory.Verify(x => x.InsertOneAsync(It.IsAny<History>(), null, CancellationToken.None));
        publisher.Verify(x => x.Publish(It.IsAny<string>(), It.IsAny<UpsertedItem<Meeting>>()));
        result.Id.Should().Be(id);
    }
}