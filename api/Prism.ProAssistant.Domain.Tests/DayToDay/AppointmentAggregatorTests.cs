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
        
        var documentId = Identifier.GenerateString();
        var attachAppointmentDocument = new AttachAppointmentDocument
        {
            Document = new BinaryDocument
            {
                Id = documentId,
                FileName = "FileName",
                Title = "Title"
            },
            StreamId = streamId
        };
        
        var detachAppointmentDocument = new DetachAppointmentDocument
        {
            DocumentId = documentId,
            StreamId = streamId
        };
        
        var appointmentColorUpdated = new AppointmentColorUpdated
        {
            BackgroundColor = Identifier.GenerateString(),
            ForeColor = Identifier.GenerateString(),
            StreamId = streamId
        };
        
        var appointmentContactUpdated = new AppointmentContactUpdated
        {
            Title = Identifier.GenerateString(),
            FirstName = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            BirthDate = Identifier.GenerateString(),
            PhoneNumber = Identifier.GenerateString(),
            StreamId = streamId
        };

        // Act and assert events
        aggregator.When(DomainEvent.FromEvent(streamId, userId, appointmentCreated));
        aggregator.State.FirstName.Should().Be("John");
        aggregator.State.LastName.Should().Be("Doe");

        aggregator.When(DomainEvent.FromEvent(streamId, userId, appointmentUpdated));
        aggregator.State.FirstName.Should().Be("Jane");
        aggregator.State.LastName.Should().Be("Doe");
        
        aggregator.When(DomainEvent.FromEvent(streamId, userId, attachAppointmentDocument));
        aggregator.State.Documents.Should().HaveCount(1);
        
        aggregator.When(DomainEvent.FromEvent(streamId, userId, detachAppointmentDocument));
        aggregator.State.Documents.Should().BeEmpty();
        
        aggregator.When(DomainEvent.FromEvent(streamId, userId, appointmentColorUpdated));
        aggregator.State.BackgroundColor.Should().Be(appointmentColorUpdated.BackgroundColor);
        aggregator.State.ForeColor.Should().Be(appointmentColorUpdated.ForeColor);
        
        aggregator.When(DomainEvent.FromEvent(streamId, userId, appointmentContactUpdated));
        aggregator.State.Title.Should().Be(appointmentContactUpdated.Title);
        aggregator.State.FirstName.Should().Be(appointmentContactUpdated.FirstName);
        aggregator.State.LastName.Should().Be(appointmentContactUpdated.LastName);
        aggregator.State.BirthDate.Should().Be(appointmentContactUpdated.BirthDate);
        aggregator.State.PhoneNumber.Should().Be(appointmentContactUpdated.PhoneNumber);

        // Assert
        aggregator.State.Id.Should().Be(streamId);

        // Assert unknown events
        aggregator.Invoking(x => x.When(DomainEvent.FromEvent(streamId, userId, new DummyEvent())))
            .Should().Throw<NotSupportedException>();
    }
}