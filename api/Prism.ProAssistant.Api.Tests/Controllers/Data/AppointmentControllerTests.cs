using FluentAssertions;
using Moq;
using Prism.Core;
using Prism.Infrastructure.Providers;
using Prism.ProAssistant.Api.Controllers.Data;
using Prism.ProAssistant.Domain.DayToDay.Appointments;
using Prism.ProAssistant.Domain.DayToDay.Appointments.Events;
using Prism.ProAssistant.Domain.DayToDay.Contacts;
using Prism.ProAssistant.Domain.DayToDay.Contacts.Events;
using Prism.ProAssistant.Storage;
using Prism.ProAssistant.Storage.Events;

namespace Prism.ProAssistant.Api.Tests.Controllers.Data;

using Domain;

public class AppointmentControllerTests
{
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