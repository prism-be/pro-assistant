// -----------------------------------------------------------------------
//  <copyright file = "BaseUpdatedServiceBusWorkerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.ProAssistant.Api.Workers;
using Prism.ProAssistant.Business.Commands;
using Prism.ProAssistant.Business.Events;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using RabbitMQ.Client;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Workers;

public class DataUpdatedServiceBusWorkerTests
{
    [Fact]
    public async Task Execute_Ok()
    {
        // Arrange
        // Arrange
        var logger = new Mock<ILogger<DataUpdatedServiceBusWorker<Meeting>>>();
        var provider = new Mock<IServiceProvider>();
        var connection = new Mock<IConnection>();
        var channel = new Mock<IModel>();
        connection.Setup(x => x.CreateModel())
            .Returns(channel.Object);
        var queue = Identifier.GenerateString();
        var workerName = Identifier.GenerateString();

        var mediator = new Mock<IMediator>();
        
        var message = new UpsertedItem<Meeting>(new Meeting(), new Meeting(), Identifier.GenerateString());
        
        // Act
        var worker = new DataUpdatedServiceBusWorker<Meeting>(logger.Object, 
            provider.Object, 
            connection.Object, 
            _ => new UpdateMeetingsColor(new Tariff(), new Tariff(), Identifier.GenerateString()), 
            queue,
            workerName);

        await worker.ProcessMessageAsync(mediator.Object, message);

        // Assert
        mediator.Verify(x => x.Send(It.IsAny<UpdateMeetingsColor>(), CancellationToken.None));
    }
}