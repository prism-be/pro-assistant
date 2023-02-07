// -----------------------------------------------------------------------
//  <copyright file = "FindOneServiceTests.cs" company = "Prism">
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

public class FindOneServiceTests
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

        // Act
        var handler = new FindOneService(organizationContext.Object);
        var result = await handler.Find<Contact>(id);

        // Assert
        result.Should().NotBeNull();
        result?.Id.Should().Be(id);
    }
}