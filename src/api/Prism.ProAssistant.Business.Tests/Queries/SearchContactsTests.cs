// -----------------------------------------------------------------------
//  <copyright file = "SearchContactsTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Queries;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Tests.Queries;

public class SearchContactsTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var organizationContext = new Mock<IOrganizationContext>();

        var collection = new Mock<IMongoCollection<Contact>>();
        organizationContext.Setup(x => x.GetCollection<Contact>())
            .Returns(collection.Object);

        var id = Identifier.GenerateString();

        var items = new List<Contact>
        {
            new()
            {
                Id = id,
                FirstName = "Simon"
            }
        };
        var cursor = new Mock<IAsyncCursor<Contact>>();
        cursor.Setup(_ => _.Current).Returns(items);
        cursor
            .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);
        cursor
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(true))
            .Returns(Task.FromResult(false));

        collection.Setup(x => x.FindAsync(It.IsAny<FilterDefinition<Contact>>(), It.IsAny<FindOptions<Contact>>(), CancellationToken.None))
            .ReturnsAsync(cursor.Object);

        var logger = new Mock<ILogger<SearchContactsHandler>>();
        var userContextAccessor = new Mock<IUserContextAccessor>();

        // Act
        var handler = new SearchContactsHandler(organizationContext.Object, logger.Object, userContextAccessor.Object);
        var result = await handler.Handle(new SearchContacts(Identifier.GenerateString(), Identifier.GenerateString(), Identifier.GenerateString(), Identifier.GenerateString()),
            CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(1);
    }

    [Fact]
    public async Task Handle_Ok_Empty()
    {
        // Arrange
        var organizationContext = new Mock<IOrganizationContext>();

        var collection = new Mock<IMongoCollection<Contact>>();
        organizationContext.Setup(x => x.GetCollection<Contact>())
            .Returns(collection.Object);

        var cursor = new Mock<IAsyncCursor<Contact>>();
        cursor.Setup(_ => _.Current).Returns(new List<Contact>());
        cursor
            .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);
        cursor
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(true))
            .Returns(Task.FromResult(false));

        collection.Setup(x => x.FindAsync(It.IsAny<FilterDefinition<Contact>>(), It.IsAny<FindOptions<Contact>>(), CancellationToken.None))
            .ReturnsAsync(cursor.Object);

        var logger = new Mock<ILogger<SearchContactsHandler>>();
        var userContextAccessor = new Mock<IUserContextAccessor>();

        // Act
        var handler = new SearchContactsHandler(organizationContext.Object, logger.Object, userContextAccessor.Object);
        var result = await handler.Handle(new SearchContacts(string.Empty, string.Empty, string.Empty, string.Empty), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(0);
    }
}