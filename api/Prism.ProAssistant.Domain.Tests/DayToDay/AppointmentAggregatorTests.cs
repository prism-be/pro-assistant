using FluentAssertions;
using Prism.Core;
using Prism.ProAssistant.Domain.DayToDay.Appointments;
using Prism.ProAssistant.Domain.DayToDay.Appointments.Events;

namespace Prism.ProAssistant.Domain.Tests.DayToDay;

public class AppointmentAggregatorTests
{
    [Fact]
    public void Happy()
    {
        // Arrange
        var streamId = Identifier.GenerateString();
        var userId = Identifier.GenerateString();

        // Act
        var aggregator = new AppointmentAggregator();
        aggregator.Init(streamId);

        var appointmentCreated = new AppointmentCreated
        {
            Appointment = new Appointment
            {
                Id = streamId,
                FirstName = "John",
                LastName = "Doe",
                Title = "Title"
            }
        };

        var appointmentUpdated = new AppointmentUpdated
        {
            Appointment = new Appointment
            {
                Id = streamId,
                FirstName = "Jane",
                LastName = "Doe",
                Title = "Title"
            }
        };

        // Act and assert events
        aggregator.When(DomainEvent.FromEvent(streamId, userId, appointmentCreated));
        aggregator.State.FirstName.Should().Be("John");
        aggregator.State.LastName.Should().Be("Doe");

        aggregator.When(DomainEvent.FromEvent(streamId, userId, appointmentUpdated));
        aggregator.State.FirstName.Should().Be("Jane");
        aggregator.State.LastName.Should().Be("Doe");

        // Assert
        aggregator.State.Id.Should().Be(streamId);

        // Assert unknown events
        aggregator.Invoking(x => x.When(DomainEvent.FromEvent(streamId, userId, new DummyEvent())))
            .Should().Throw<NotSupportedException>();
    }
}