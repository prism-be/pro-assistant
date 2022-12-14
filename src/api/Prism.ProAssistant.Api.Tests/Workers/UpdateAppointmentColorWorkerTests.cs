// -----------------------------------------------------------------------
//  <copyright file = "UpdateAppointmentColorWorkerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.ProAssistant.Api.Workers;
using Prism.ProAssistant.Business.Commands;
using Prism.ProAssistant.Business.Events;
using Prism.ProAssistant.Business.Security;
using RabbitMQ.Client;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Workers;

public class UpdateAppointmentColorWorkerTests
{
    [Fact]
    public async Task Execute_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<UpdateAppointmentColorWorker>>();
        var serviceProvider = new Mock<IServiceProvider>();
        var connection = new Mock<IConnection>();
        var mediator = new Mock<IMediator>();

        // Act
        var worker = new UpdateAppointmentColorWorker(logger.Object, serviceProvider.Object, connection.Object);
        await worker.ProcessMessageAsync(mediator.Object, new UpsertResult(Identifier.GenerateString(), Identifier.GenerateString()));

        // Assert
        worker.Queue.Should().Be(Topics.Tariffs.Updated);
        worker.WorkerName.Should().Be("UpdateAppointmentColorWorker");
        mediator.Verify(x => x.Send(It.IsAny<UpdateAppointmentsColor>(), CancellationToken.None));
    }
}