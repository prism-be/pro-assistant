// -----------------------------------------------------------------------
//  <copyright file = "DeleteDocumentServiceTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;
using Prism.ProAssistant.UnitTesting;
using Prism.ProAssistant.UnitTesting.Extensions;
using Xunit;

namespace Prism.ProAssistant.Documents.Tests;

public class DeleteDocumentServiceTests
{
    [Fact]
    public async Task Handle_NotFound()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();
        var appointmentId = ObjectId.GenerateNewId().ToString();

        var organization = new Mock<IOrganizationContext>();
        var bucket = organization.SetupBucket();
        organization.SetupCollection(new Appointment
        {
            Id = appointmentId,
            Documents = new List<BinaryDocument>(),
            FirstName = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            Title = Identifier.GenerateString()
        });

        var logger = new Mock<ILogger<DeleteDocumentService>>();

        var user = new Mock<User>();

        // Act
        var service = new DeleteDocumentService(logger.Object, organization.Object, user.Object);
        await service.Delete(id, appointmentId);

        // Assert
        bucket.Verify(x => x.DeleteAsync(It.IsAny<ObjectId>(), CancellationToken.None), Times.Never);
        logger.VerifyLog(LogLevel.Warning);
    }

    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();
        var appointmentId = ObjectId.GenerateNewId().ToString();

        var organization = new Mock<IOrganizationContext>();
        var bucket = organization.SetupBucket();
        organization.SetupCollection(new Appointment
        {
            Id = appointmentId,
            FirstName = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            Title = Identifier.GenerateString(),
            Documents = new List<BinaryDocument>
            {
                new()
                {
                    Id = id,
                    FileName = Identifier.GenerateString(),
                    Title = Identifier.GenerateString()
                }
            }
        });

        var logger = new Mock<ILogger<DeleteDocumentService>>();

        var user = new Mock<User>();

        // Act
        var handler = new DeleteDocumentService(logger.Object, organization.Object, user.Object);
        await handler.Delete(id, appointmentId);

        // Assert
        bucket.Verify(x => x.DeleteAsync(It.IsAny<ObjectId>(), CancellationToken.None), Times.Once);
        logger.VerifyLog(LogLevel.Information, Times.AtLeastOnce());
    }
}