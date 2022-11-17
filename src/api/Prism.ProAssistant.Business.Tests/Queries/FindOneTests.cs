// -----------------------------------------------------------------------
//  <copyright file = "FindManyTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;
using Moq;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Queries;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Tests.Queries
{
    public class FindOneTests
    {
        [Fact]
        public async Task Handle_Ok()
        {
            // Arrange
            var organisationContext = new Mock<IOrganizationContext>();

            var collection = new Mock<IMongoCollection<Patient>>();
            organisationContext.Setup(x => x.GetCollection<Patient>())
                .Returns(collection.Object);

            var id = Identifier.GenerateString();

            var items = new List<Patient>
            {
                new()
                {
                    Id = id,
                    FirstName = "Simon"
                }
            };
            var cursor = new Mock<IAsyncCursor<Patient>>();
            cursor.Setup(_ => _.Current).Returns(items);
            cursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            cursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                .Returns(Task.FromResult(false));

            collection.Setup(x => x.FindAsync(It.IsAny<FilterDefinition<Patient>>(), It.IsAny<FindOptions<Patient>>(), CancellationToken.None))
                .ReturnsAsync(cursor.Object);

            // Act
            var handler = new FindOneHandler<Patient>(organisationContext.Object);
            var result = await handler.Handle(new FindOne<Patient>(id), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result?.Id.Should().Be(id);
        }
    }
}