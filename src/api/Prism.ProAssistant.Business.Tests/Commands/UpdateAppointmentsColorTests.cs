// -----------------------------------------------------------------------
//  <copyright file = "UpdateAppointmentsColorTests.cs" company = "Prism">
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
using Prism.ProAssistant.UnitTesting.Extensions;

namespace Prism.ProAssistant.Business.Tests.Commands;

public class UpdateAppointmentsColorTests
{
    [Fact]
    public async Task Handle_No_Tariff()
    {
        // Arrange
        var logger = new Mock<ILogger<UpdateAppointmentsColorHandler>>();
        var organizationContext = new Mock<IOrganizationContext>();
        var request = new UpdateAppointmentsColor(Identifier.GenerateString(), Identifier.GenerateString());

        var tariffCollection = new Mock<IMongoCollection<Tariff>>();
        tariffCollection.SetupCollectionFindEmpty();

        organizationContext.Setup(x => x.GetCollection<Tariff>())
            .Returns(tariffCollection.Object);

        // Act
        var handler = new UpdateAppointmentsColorHandler(logger.Object, organizationContext.Object);
        await handler.Handle(request, CancellationToken.None);

        // Assert
        logger.VerifyLog(LogLevel.Warning, Times.Once());
    }

    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<UpdateAppointmentsColorHandler>>();
        var organizationContext = new Mock<IOrganizationContext>();
        var request = new UpdateAppointmentsColor(Identifier.GenerateString(), Identifier.GenerateString());

        var tariffCollection = new Mock<IMongoCollection<Tariff>>();
        tariffCollection.SetupCollection(new Tariff
        {
            Id = Identifier.GenerateString(),
            Name = Identifier.GenerateString()
        });

        organizationContext.Setup(x => x.GetCollection<Tariff>())
            .Returns(tariffCollection.Object);

        var appointmentCollection = new Mock<IMongoCollection<Appointment>>();
        appointmentCollection.SetupCollection(new Appointment
        {
            Id = Identifier.GenerateString(),
            FirstName = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            Title = Identifier.GenerateString()
        });

        organizationContext.Setup(x => x.GetCollection<Appointment>())
            .Returns(appointmentCollection.Object);

        // Act
        var handler = new UpdateAppointmentsColorHandler(logger.Object, organizationContext.Object);
        await handler.Handle(request, CancellationToken.None);

        // Assert
        logger.VerifyLog(LogLevel.Warning, Times.Never());
        logger.VerifyLog(LogLevel.Information, Times.AtLeastOnce());
    }
}