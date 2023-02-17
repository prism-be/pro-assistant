// -----------------------------------------------------------------------
//  <copyright file = "FindManyServiceTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Services;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Tests.Services;

public class FindManyServiceTests
{

    [Fact]
    public async Task Find_Fitlered()
    {
        // Arrange
        var organizationContext = new Mock<IOrganizationContext>();

        var collection = new Mock<IMongoCollection<Contact>>();
        organizationContext.Setup(x => x.GetCollection<Contact>())
            .Returns(collection.Object);

        var items = new List<Contact>
        {
            new()
            {
                Id = Identifier.GenerateString(),
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

        var filter = Builders<Contact>.Filter.Eq("FirstName", "Simon");

        collection.Setup(x => x.FindAsync<Contact>(filter, null, CancellationToken.None))
            .ReturnsAsync(cursor.Object);

        // Act
        var handler = new FindManyService(organizationContext.Object);
        var result = await handler.Find(filter);

        // Assert
        result.Count.Should().Be(1);
    }

    [Fact]
    public async Task Find_Ok()
    {
        // Arrange
        var organizationContext = new Mock<IOrganizationContext>();

        var collection = new Mock<IMongoCollection<Contact>>();
        organizationContext.Setup(x => x.GetCollection<Contact>())
            .Returns(collection.Object);

        var items = new List<Contact>
        {
            new()
            {
                Id = Identifier.GenerateString(),
                FirstName = "Simon"
            },
            new()
            {
                Id = Identifier.GenerateString(),
                FirstName = "John"
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

        collection.Setup(x => x.FindAsync<Contact>(Builders<Contact>.Filter.Empty, null, CancellationToken.None))
            .ReturnsAsync(cursor.Object);

        // Act
        var handler = new FindManyService(organizationContext.Object);
        var result = await handler.Find<Contact>();

        // Assert
        result.Count.Should().Be(2);
    }
}