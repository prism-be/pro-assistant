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
        var logger = new Mock<ILogger<UpsertOneHandler<Meeting>>>();
        var organizationContext = new Mock<IOrganizationContext>();
        var userContextAccessor = new Mock<IUserContextAccessor>();

        var collection = new Mock<IMongoCollection<Meeting>>();
        organizationContext.Setup(x => x.GetCollection<Meeting>())
            .Returns(collection.Object);

        var id = Identifier.GenerateString();
        var meeting = new Meeting
        {
            Id = id
        };

        // Act
        var request = new UpsertOne<Meeting>(meeting);
        var handler = new UpsertOneHandler<Meeting>(logger.Object, organizationContext.Object, userContextAccessor.Object);
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        collection.Verify(x => x.FindOneAndReplaceAsync(It.IsAny<FilterDefinition<Meeting>>(), meeting, It.IsAny<FindOneAndReplaceOptions<Meeting>>(), CancellationToken.None));
        result.Id.Should().Be(id);
    }
}