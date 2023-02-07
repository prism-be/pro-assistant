// // -----------------------------------------------------------------------
// //  <copyright file = "BaseServiceBusWorkerTests.cs" company = "Prism">
// //  Copyright (c) Prism.All rights reserved.
// //  </copyright>
// // -----------------------------------------------------------------------
//
// using MediatR;
// using Microsoft.Extensions.Logging;
// using Moq;
// using Prism.ProAssistant.Api.Workers;
// using Prism.ProAssistant.Business.Security;
// using RabbitMQ.Client;
// using RabbitMQ.Client.Events;
// using Xunit;
//
// namespace Prism.ProAssistant.Api.Tests.Workers;
//
// public class DummyServiceBusWorker : BaseServiceBusWorker<string>
// {
//     public DummyServiceBusWorker(ILogger logger, IServiceProvider serviceProvider, IConnection? connection) : base(logger, serviceProvider, connection)
//     {
//     }
//
//     public override string Queue { get; } = Identifier.GenerateString();
//     public override string WorkerName { get; } = "DummyServiceBusWorker";
//
//     public async Task ExecuteTest()
//     {
//         await ExecuteAsync(CancellationToken.None);
//     }
//
//     public override Task ProcessMessageAsync(IMediator mediator, string payload)
//     {
//         return Task.CompletedTask;
//     }
// }
//
// public class BaseServiceBusWorkerTests
// {
//     [Fact]
//     public async Task ExecuteAsync_Ok()
//     {
//         // Arrange
//         var logger = new Mock<ILogger<DummyServiceBusWorker>>();
//         var provider = new Mock<IServiceProvider>();
//         var connection = new Mock<IConnection>();
//         var channel = new Mock<IModel>();
//         connection.Setup(x => x.CreateModel())
//             .Returns(channel.Object);
//
//         // Act
//         using var worker = new DummyServiceBusWorker(logger.Object, provider.Object, connection.Object);
//         await worker.ExecuteTest();
//
//         // Assert
//         channel.Verify(x => x.BasicConsume("workers/DummyServiceBusWorker", false, "", false, false, null, It.IsAny<EventingBasicConsumer>()), Times.Once);
//     }
// }