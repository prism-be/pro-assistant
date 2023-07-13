namespace Prism.ProAssistant.Api.Tests.Controllers;

using Api.Controllers;
using Core;
using Domain;
using Domain.Accounting.Reporting;
using Domain.DayToDay.Appointments;
using Domain.DayToDay.Contacts;
using Infrastructure.Providers;
using Microsoft.Extensions.Logging;
using Moq;
using Storage;
using Storage.Events;

public class MaintenanceControllerTests
{
    [Fact]
    public async Task RebuildAccountingPeriod_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<MaintenanceController>>();
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();
        var stateProvider = new Mock<IStateProvider>();
        var container = new Mock<IStateContainer<AccountingReportingPeriod>>();
        stateProvider.Setup(x => x.GetContainerAsync<AccountingReportingPeriod>()).ReturnsAsync(container.Object);

        queryService.Setup(x => x.ListAsync<Appointment>())
            .ReturnsAsync(new List<Appointment>
            {
                new() { Id = Identifier.GenerateString(), StartDate = new DateTime(2021, 1, 1), Price = 100, State = (int)AppointmentState.Done },
                new() { Id = Identifier.GenerateString(), StartDate = new DateTime(2021, 2, 1), Price = 100, State = (int)AppointmentState.Done }
            });

        // Act
        var maintenanceController = new MaintenanceController(logger.Object, queryService.Object, eventStore.Object, stateProvider.Object);
        await maintenanceController.RebuildAccountingPeriods();

        // Assert
        container.Verify(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<AccountingReportingPeriod>()), Times.Exactly(2));
    }

    [Fact]
    public async Task RebuildProjections_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<MaintenanceController>>();
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();
        var streams = new List<string> { "stream1", "stream2" };
        queryService.Setup(x => x.DistinctAsync<DomainEvent, string>(nameof(DomainEvent.StreamId), It.IsAny<Filter[]>())).ReturnsAsync(streams);

        // Act
        var maintenanceController = new MaintenanceController(logger.Object, queryService.Object, eventStore.Object, new Mock<IStateProvider>().Object);
        await maintenanceController.RebuildProjections();

        // Assert
        eventStore.Verify(x => x.Persist<Contact>("stream1"), Times.Once);
        eventStore.Verify(x => x.Persist<Contact>("stream2"), Times.Once);
    }
}