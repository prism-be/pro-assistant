// -----------------------------------------------------------------------
//  <copyright file = "FindManyTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Queries;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Tests.Queries
{
    public class FindManyTests
    {
        [Fact]
        public async Task Handle_Ok()
        {
            // Arrange
            var organisationContext = new Mock<IOrganizationContext>();

            var collection = new Mock<IMongoCollection<Patient>>();
            organisationContext.Setup(x => x.GetCollection<Patient>())
                .Returns(collection.Object);

            var items = new List<Patient>
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

            collection.Setup(x => x.FindAsync<Patient>(Builders<Patient>.Filter.Empty, null, CancellationToken.None))
                .ReturnsAsync(cursor.Object);

            // Act
            var handler = new FindManyHandler<Patient>(organisationContext.Object);
            var result = await handler.Handle(new FindMany<Patient>(), CancellationToken.None);

            // Assert
            result.Count.Should().Be(2);
        }
    }
}