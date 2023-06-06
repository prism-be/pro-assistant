using FluentAssertions;
using Prism.Core;
using Prism.ProAssistant.Domain.DayToDay.Appointments;
using Prism.ProAssistant.Domain.DayToDay.Appointments.Events;

namespace Prism.ProAssistant.Domain.Tests.DayToDay;

using System.Diagnostics;
using Domain.DayToDay.Contacts;
using Moq;

public class AppointmentAggregatorTests
{
    [Fact]
    public async Task Happy()
    {
        // Arrange
        var streamId = Identifier.GenerateString();
        var userId = Identifier.GenerateString();
        
        var hydrator = new Mock<IHydrator>();
        var contact1 = new Contact
        {
            Id = Identifier.GenerateString(),
            FirstName = "John",
            LastName = "Doe",
            Title = "Title"
        };
        
        var contact2 = new Contact
        {
            Id = Identifier.GenerateString(),
            FirstName = "Jane",
            LastName = "Doe",
            Title = "Title"
        };
        
        hydrator.Setup(x => x.Hydrate<Contact>(contact1.Id)).ReturnsAsync(contact1);
        hydrator.Setup(x => x.Hydrate<Contact>(contact2.Id)).ReturnsAsync(contact2);

        // Act
        var aggregator = new AppointmentAggregator(hydrator.Object);
        aggregator.Init(streamId);

        var appointmentCreated = new AppointmentCreated
        {
            Appointment = new Appointment
            {
                Id = streamId,
                ContactId = contact1.Id,
            }
        };

        var appointmentUpdated = new AppointmentUpdated
        {
            Appointment = new Appointment
            {
                Id = streamId,
                ContactId = contact2.Id,
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
        
        var closeAppointment = new AppointmentClosed
        {
            Id = streamId,
            Payment = (int)PaymentTypes.Wire,
            State = (int)AppointmentState.Done,
            PaymentDate = DateTime.UtcNow
        };
        
        var undoCloseAppointment = new AppointmentClosed
        {
            Id = streamId,
            Payment = (int)PaymentTypes.Unpayed,
            State = (int)AppointmentState.Confirmed,
            PaymentDate = DateTime.UtcNow
        };

        var cancelAppointment = new AppointmentUpdated
        {
            Appointment = new Appointment
            {
                Id = streamId,
                ContactId = contact2.Id,
                State = (int)AppointmentState.Canceled
            }
        };

        // Act and assert events
        await aggregator.When(DomainEvent.FromEvent(streamId, userId, appointmentCreated));
        await aggregator.Complete();
        Debug.Assert(aggregator.State != null, "aggregator.State != null");
        aggregator.State.FirstName.Should().Be("John");
        aggregator.State.LastName.Should().Be("Doe");

        await aggregator.When(DomainEvent.FromEvent(streamId, userId, appointmentUpdated));
        await aggregator.Complete();
        aggregator.State.FirstName.Should().Be("Jane");
        aggregator.State.LastName.Should().Be("Doe");

        await aggregator.When(DomainEvent.FromEvent(streamId, userId, attachAppointmentDocument));
        aggregator.State.Documents.Should().HaveCount(1);

        await aggregator.When(DomainEvent.FromEvent(streamId, userId, detachAppointmentDocument));
        aggregator.State.Documents.Should().BeEmpty();

        aggregator.State.Id.Should().Be(streamId);
        
        await aggregator.When(DomainEvent.FromEvent(streamId, userId, closeAppointment));
        aggregator.State.State.Should().Be((int)AppointmentState.Done);
        aggregator.State.Payment.Should().Be((int)PaymentTypes.Wire);
        aggregator.State.PaymentDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        
        await aggregator.When(DomainEvent.FromEvent(streamId, userId, undoCloseAppointment));
        aggregator.State.State.Should().Be((int)AppointmentState.Confirmed);
        aggregator.State.Payment.Should().Be((int)PaymentTypes.Unpayed);
        aggregator.State.PaymentDate.Should().BeNull();
        
        await aggregator.When(DomainEvent.FromEvent(streamId, userId, cancelAppointment));
        aggregator.State.Should().BeNull();

        // Assert unknown events
        await aggregator.Invoking(x => x.When(DomainEvent.FromEvent(streamId, userId, new DummyEvent())))
            .Should().ThrowAsync<NotSupportedException>();
    }
}