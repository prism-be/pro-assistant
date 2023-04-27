namespace Prism.ProAssistant.Storage.Tests.Effects;

using Core;
using Domain;
using Domain.DayToDay.Appointments;
using Domain.DayToDay.Contacts;
using Domain.DayToDay.Contacts.Events;
using Infrastructure.Authentication;
using Infrastructure.Providers;
using Microsoft.Extensions.Logging;
using Moq;
using Storage.Effects;
using Storage.Events;

public class RefreshAppointmentWhenContactChangeTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<RefreshAppointmentWhenContactChange>>();
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        var @event = new ContactUpdated
        {
            Contact = new Contact
            {
                Id = Identifier.GenerateString()
            }
        };

        queryService.Setup(x => x.DistinctAsync<Appointment, string>(nameof(Appointment.Id), new Filter(nameof(Appointment.ContactId), @event.Contact.Id, FilterOperator.Equal)))
            .ReturnsAsync(new List<string>
            {
                Identifier.GenerateString()
            });

        // Act
        var effect = new RefreshAppointmentWhenContactChange(logger.Object, queryService.Object, eventStore.Object);
        await effect.Handle(DomainEvent.FromEvent(@event.StreamId, Identifier.GenerateString(), @event));

        // Assert
        eventStore.Verify(x => x.Persist<Appointment>(It.IsAny<string>()), Times.Once);
    }
}