// -----------------------------------------------------------------------
//  <copyright file = "SearchMeetingsTests.cs" company = "Prism">
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

namespace Prism.ProAssistant.Business.Tests.Queries
{
    public class SearchPatientsTests
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

            var logger = new Mock<ILogger<SearchPatientsHandler>>();
            var userContextAccessor = new Mock<IUserContextAccessor>();

            // Act
            var handler = new SearchPatientsHandler(organisationContext.Object, logger.Object, userContextAccessor.Object);
            var result = await handler.Handle(new SearchPatients(Identifier.GenerateString(), Identifier.GenerateString(), Identifier.GenerateString(), Identifier.GenerateString()), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
        }
        
        [Fact]
        public async Task Handle_Ok_Empty()
        {
            // Arrange
            var organisationContext = new Mock<IOrganizationContext>();

            var collection = new Mock<IMongoCollection<Patient>>();
            organisationContext.Setup(x => x.GetCollection<Patient>())
                .Returns(collection.Object);

            var cursor = new Mock<IAsyncCursor<Patient>>();
            cursor.Setup(_ => _.Current).Returns(new List<Patient>());
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

            var logger = new Mock<ILogger<SearchPatientsHandler>>();
            var userContextAccessor = new Mock<IUserContextAccessor>();

            // Act
            var handler = new SearchPatientsHandler(organisationContext.Object, logger.Object, userContextAccessor.Object);
            var result = await handler.Handle(new SearchPatients(string.Empty, string.Empty, string.Empty, string.Empty), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(0);
        }
    }
}