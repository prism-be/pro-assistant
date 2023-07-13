namespace Prism.ProAssistant.Storage.Tests.Effects;

using Core;
using Domain;
using Domain.Accounting.Reporting;
using Domain.DayToDay.Appointments;
using Domain.DayToDay.Appointments.Events;
using Infrastructure.Providers;
using Microsoft.Extensions.Logging;
using Moq;
using Storage.Effects;

public class ProjectAccountingPeriodWhenAppointmentUpdatedTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<ProjectAccountingPeriodWhenAppointmentUpdated>>();
        var queryService = new Mock<IQueryService>();
        var stateProvider = new Mock<IStateProvider>();
        var container = new Mock<IStateContainer<AccountingReportingPeriod>>();
        stateProvider.Setup(x => x.GetContainerAsync<AccountingReportingPeriod>()).ReturnsAsync(container.Object);

        var id = Identifier.GenerateString();

        var appointment = new Appointment
        {
            Id = id
        };
        
        var @event = DomainEvent.FromEvent(id, Identifier.GenerateString(), new AppointmentUpdated
        {
            StreamId = id,
            Appointment = appointment
        });
        
        queryService.Setup(x => x.SingleAsync<Appointment>(id)).ReturnsAsync(appointment);

        // Act
        var projectAccountingPeriod = new ProjectAccountingPeriodWhenAppointmentUpdated(logger.Object, queryService.Object, stateProvider.Object);
        await projectAccountingPeriod.Handle(@event);

        // Assert
        queryService.Verify(x => x.SingleAsync<Appointment>(id), Times.Once);
        queryService.Verify(x => x.SearchAsync<Appointment>(It.IsAny<Filter>(), It.IsAny<Filter>()), Times.Once);
        container.Verify(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<AccountingReportingPeriod>()), Times.Once);
    }
}