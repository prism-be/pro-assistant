namespace Prism.ProAssistant.Storage.Tests.Effects;

using Core;
using Domain;
using Domain.Configuration.Tariffs;
using Domain.Configuration.Tariffs.Events;
using Domain.DayToDay.Appointments;
using Domain.DayToDay.Contacts;
using Domain.DayToDay.Contacts.Events;
using Infrastructure.Authentication;
using Infrastructure.Providers;
using Microsoft.Extensions.Logging;
using Moq;
using Storage.Effects;
using Storage.Events;

public class RefreshAppointmentWhenTariffChangeTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<RefreshAppointmentWhenTariffChange>>();
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        var @event = new TariffUpdated
        {
            Tariff = new Tariff
            {
                Id = Identifier.GenerateString(),
                Name = Identifier.GenerateString()
            }
        };

        queryService.Setup(x => x.DistinctAsync<Appointment, string>(nameof(Appointment.Id), new Filter(nameof(Appointment.TypeId), @event.Tariff.Id, FilterOperator.Equal)))
            .ReturnsAsync(new List<string>
            {
                Identifier.GenerateString()
            });

        var context = new EventContext<Tariff>
        {
            Event = DomainEvent.FromEvent(@event.StreamId,
                Identifier.GenerateString(),
                @event),
            CurrentState = new Tariff
            {
                Id = @event.Tariff.Id,
                Name = Identifier.GenerateString(),
                BackgroundColor = Identifier.GenerateString()
            },
            Context = new UserOrganization()
        };
        
        // Act
        var effect = new RefreshAppointmentWhenTariffChange(logger.Object, queryService.Object, eventStore.Object);
        await effect.Handle(context);

        // Assert
        eventStore.Verify(x => x.Persist<Appointment>(It.IsAny<string>()), Times.Once);
    }
}