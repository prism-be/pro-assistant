﻿namespace Prism.ProAssistant.Api.Tests.Controllers.Data;

using Api.Controllers.Data;
using Core;
using Domain;
using Domain.DayToDay.Appointments;
using Domain.DayToDay.Appointments.Events;
using Domain.DayToDay.Contacts;
using Domain.DayToDay.Contacts.Events;
using FluentAssertions;
using Infrastructure.Providers;
using Models;
using Moq;
using Storage;
using Storage.Events;

public class AppointmentControllerTests
{
    [Fact]
    public void Close_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        // Act
        var controller = new AppointmentController(queryService.Object, eventStore.Object);
        var result = controller.Close(new AppointmentClosing
        {
            Id = Identifier.GenerateString(),
            Payment = (int)PaymentTypes.Cash,
            State = (int)AppointmentState.Done
        });

        // Assert
        result.Should().NotBeNull();
        eventStore.Verify(x => x.RaiseAndPersist<Appointment>(It.Is<AppointmentClosed>(ac => ac.PaymentDate != null && ac.Payment == (int)PaymentTypes.Cash && ac.State == (int)AppointmentState.Done)),
            Times.Once);
    }

    [Fact]
    public async Task Insert_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        eventStore.Setup(x => x.RaiseAndPersist<Contact>(It.IsAny<ContactCreated>())).ReturnsAsync(new UpsertResult(Identifier.GenerateString()));

        // Act
        var controller = new AppointmentController(queryService.Object, eventStore.Object);
        await controller.Insert(new Appointment
        {
            FirstName = Identifier.GenerateString(),
            Id = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            Title = Identifier.GenerateString()
        });

        // Assert
        eventStore.Verify(x => x.RaiseAndPersist<Appointment>(It.IsAny<AppointmentCreated>()), Times.Once);
    }

    [Fact]
    public async Task List_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        // Act
        var controller = new AppointmentController(queryService.Object, eventStore.Object);
        await controller.List();

        // Assert
        queryService.Verify(x => x.ListAsync<Appointment>(), Times.Once);
    }

    [Fact]
    public async Task Search_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        // Act
        var controller = new AppointmentController(queryService.Object, eventStore.Object);
        await controller.Search(Array.Empty<Filter>());

        // Assert
        queryService.Verify(x => x.SearchAsync<Appointment>(Array.Empty<Filter>()), Times.Once);
    }

    [Fact]
    public async Task Single_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();
        var id = Identifier.GenerateString();

        // Act
        var controller = new AppointmentController(queryService.Object, eventStore.Object);
        await controller.Single(id);

        // Assert
        queryService.Verify(x => x.SingleOrDefaultAsync<Appointment>(id), Times.Once);
    }

    [Fact]
    public async Task Update_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        var appointment = new Appointment
        {
            FirstName = Identifier.GenerateString(),
            Id = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            Title = Identifier.GenerateString(),
            ContactId = Identifier.GenerateString()
        };

        // Act
        var controller = new AppointmentController(queryService.Object, eventStore.Object);
        await controller.Update(appointment);

        // Assert
        eventStore.Verify(x => x.RaiseAndPersist<Appointment>(It.Is<AppointmentUpdated>(y => y.Appointment == appointment)), Times.Once);
    }
}