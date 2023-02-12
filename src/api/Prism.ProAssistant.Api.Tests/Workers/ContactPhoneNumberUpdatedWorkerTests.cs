// -----------------------------------------------------------------------
//  <copyright file = "ContactBirthDateUpdatedWorkerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.ProAssistant.Api.Workers;
using Prism.ProAssistant.Business.Events;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Services;
using RabbitMQ.Client;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Workers;

public class ContactPhoneNumberpdatedWorkerTests
{
    [Fact]
    public async Task ProcessMessageAsync_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<ContactPhoneNumberUpdatedWorker>>();

        var crudService = new Mock<ICrudService>();
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(x => x.GetService(typeof(ICrudService)))
            .Returns(crudService.Object);

        var connection = new Mock<IConnection>();

        var user = new User();
        var propertyUpdated = new PropertyUpdated(
            Identifier.GenerateString(),
            "BirthDate",
            nameof(String),
            DateTime.Now);

        // Act
        var worker = new ContactPhoneNumberUpdatedWorker(logger.Object, serviceProvider.Object, connection.Object);
        await worker.ProcessMessageAsync(serviceProvider.Object, new Event<PropertyUpdated>(user, propertyUpdated));

        // Assert
        crudService.Verify(x => x.UpdateManyProperty<Appointment>(nameof(Appointment.ContactId), propertyUpdated.Id, nameof(Appointment.PhoneNumber), propertyUpdated.Value), Times.Once);
        worker.WorkerName.Should().Be("ContactPhoneNumberUpdatedWorker");
        worker.Queue.Should().Be("Property.Updated.Contact.PhoneNumber");
    }
}