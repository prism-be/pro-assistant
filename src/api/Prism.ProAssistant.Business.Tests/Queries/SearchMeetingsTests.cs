// -----------------------------------------------------------------------
//  <copyright file = "SearchMeetingsTests.cs" company = "Prism">
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
    public class SearchMeetingsTests
    {
        [Fact]
        public async Task Handle_Ok()
        {
            // Arrange
            var organisationContext = new Mock<IOrganizationContext>();

            var collection = new Mock<IMongoCollection<Meeting>>();
            organisationContext.Setup(x => x.GetCollection<Meeting>())
                .Returns(collection.Object);

            var id = Identifier.GenerateString();

            var items = new List<Meeting>
            {
                new()
                {
                    Id = id,
                    FirstName = "Simon"
                }
            };
            var cursor = new Mock<IAsyncCursor<Meeting>>();
            cursor.Setup(_ => _.Current).Returns(items);
            cursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            cursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                .Returns(Task.FromResult(false));

            collection.Setup(x => x.FindAsync(It.IsAny<FilterDefinition<Meeting>>(), It.IsAny<FindOptions<Meeting>>(), CancellationToken.None))
                .ReturnsAsync(cursor.Object);

            // Act
            var handler = new SearchMeetingsHandler(organisationContext.Object);
            var result = await handler.Handle(new SearchMeetings(DateTime.Now, DateTime.Now.AddDays(7)), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
        }
    }
}