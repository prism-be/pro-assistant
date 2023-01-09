// -----------------------------------------------------------------------
//  <copyright file = "SearchAppointmentsTests.cs" company = "Prism">
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
    public class SearchAppointmentsTests
    {
        [Fact]
        public async Task Handle_Ok()
        {
            // Arrange
            var organizationContext = new Mock<IOrganizationContext>();

            var collection = new Mock<IMongoCollection<Appointment>>();
            organizationContext.Setup(x => x.GetCollection<Appointment>())
                .Returns(collection.Object);

            var id = Identifier.GenerateString();

            var items = new List<Appointment>
            {
                new()
                {
                    Id = id,
                    FirstName = "Simon"
                }
            };
            var cursor = new Mock<IAsyncCursor<Appointment>>();
            cursor.Setup(_ => _.Current).Returns(items);
            cursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            cursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                .Returns(Task.FromResult(false));

            collection.Setup(x => x.FindAsync(It.IsAny<FilterDefinition<Appointment>>(), It.IsAny<FindOptions<Appointment>>(), CancellationToken.None))
                .ReturnsAsync(cursor.Object);

            // Act
            var handler = new SearchAppointmentsHandler(organizationContext.Object);
            var result = await handler.Handle(new SearchAppointments(DateTime.Now, DateTime.Now.AddDays(7), null), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
        }
    }
}