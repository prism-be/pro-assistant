// -----------------------------------------------------------------------
//  <copyright file = "RemoveOneTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Business.Commands;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Tests.Commands;

public class RemoveOneTests
{
    [Fact]
    public async Task RemoveOneHandler_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<RemoveOneHandler<Meeting>>>();
        var organizationContext = new Mock<IOrganizationContext>();
        var userContextAccessor = new Mock<IUserContextAccessor>();

        var collection = new Mock<IMongoCollection<Meeting>>();
        organizationContext.Setup(x => x.GetCollection<Meeting>())
            .Returns(collection.Object);

        // Act
        var request = new RemoveOne<Meeting>(Identifier.GenerateString());
        var handler = new RemoveOneHandler<Meeting>(logger.Object, organizationContext.Object, userContextAccessor.Object);
        await handler.Handle(request, CancellationToken.None);

        // Assert
        collection.Verify(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<Meeting>>(), CancellationToken.None));
    }
}