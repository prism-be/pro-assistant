// -----------------------------------------------------------------------
//  <copyright file = "UpdateMeetingsColorTests.cs" company = "Prism">
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

public class UpdateMeetingsColorTests
{
    [Fact]
    public async Task Handle_No_Tariff()
    {
        // Arrange
        var logger = new Mock<ILogger<UpdateMeetingsColorHandler>>();
        var organizationContext = new Mock<IOrganizationContext>();

        var previous = new Tariff
        {
            Id = Identifier.GenerateString(),
            BackgroundColor = Identifier.GenerateString(),
            ForeColor = Identifier.GenerateString()
        };
        
        var current = new Tariff
        {
            Id = Identifier.GenerateString(),
            BackgroundColor = previous.BackgroundColor,
            ForeColor = previous.ForeColor
        };
        
        
        var request = new UpdateMeetingsColor(previous, current, Identifier.GenerateString());

        var tariffCollection = new Mock<IMongoCollection<Tariff>>();
        tariffCollection.SetupCollectionFindEmpty();
        
        organizationContext.Setup(x => x.GetCollection<Tariff>())
            .Returns(tariffCollection.Object);
        
        // Act
        var handler = new UpdateMeetingsColorHandler(logger.Object, organizationContext.Object);
        await handler.Handle(request, CancellationToken.None);

        // Assert
        logger.VerifyLog(LogLevel.Information, Times.Once());
    }
    
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<UpdateMeetingsColorHandler>>();
        var organizationContext = new Mock<IOrganizationContext>();
        
        var previous = new Tariff
        {
            Id = Identifier.GenerateString()
        };
        
        var current = new Tariff
        {
            Id = Identifier.GenerateString()
        };
        
        
        var request = new UpdateMeetingsColor(previous, current, Identifier.GenerateString());

        var tariffCollection = new Mock<IMongoCollection<Tariff>>();
        tariffCollection.SetupCollection(new Tariff() { Id = Identifier.GenerateString() });
        
        organizationContext.Setup(x => x.GetCollection<Tariff>())
            .Returns(tariffCollection.Object);
        
        var meetingCollection = new Mock<IMongoCollection<Meeting>>();
        meetingCollection.SetupCollection(new Meeting() { Id = Identifier.GenerateString() });
        
        organizationContext.Setup(x => x.GetCollection<Meeting>())
            .Returns(meetingCollection.Object);
        
        // Act
        var handler = new UpdateMeetingsColorHandler(logger.Object, organizationContext.Object);
        await handler.Handle(request, CancellationToken.None);

        // Assert
        logger.VerifyLog(LogLevel.Warning, Times.Never());
        logger.VerifyLog(LogLevel.Information, Times.AtLeastOnce());
    }
}