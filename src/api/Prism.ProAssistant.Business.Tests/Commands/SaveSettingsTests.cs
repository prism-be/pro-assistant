// -----------------------------------------------------------------------
//  <copyright file = "SaveSettingsTests.cs" company = "Prism">
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

public class SaveSettingsTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var existingId = Identifier.GenerateString();

        var logger = new Mock<ILogger<SaveSettingsHandler>>();
        var organizationContext = new Mock<IOrganizationContext>();
        var userContextAccessor = new Mock<IUserContextAccessor>();

        var collection = new Mock<IMongoCollection<Setting>>();
        organizationContext.Setup(x => x.GetCollection<Setting>())
            .Returns(collection.Object);

        collection.SetupCollection(new Setting
        {
            Id = existingId
        });

        var collectionHistory = new Mock<IMongoCollection<History>>();
        organizationContext.Setup(x => x.GetCollection<History>())
            .Returns(collectionHistory.Object);

        var request = new SaveSettings(new List<Setting>
        {
            new()
            {
                Id = Identifier.GenerateString(),
                Value = Identifier.GenerateString()
            },
            new()
            {
                Id = Identifier.GenerateString(),
                Value = Identifier.GenerateString()
            }
        });

        // Act
        var handler = new SaveSettingsHandler(logger.Object, organizationContext.Object, userContextAccessor.Object);
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        collection.Verify(x => x.FindOneAndReplaceAsync(It.IsAny<FilterDefinition<Setting>>(), It.IsAny<Setting>(), It.IsAny<FindOneAndReplaceOptions<Setting>>(), CancellationToken.None));
        collectionHistory.Verify(x => x.InsertOneAsync(It.IsAny<History>(), null, CancellationToken.None));
    }
}