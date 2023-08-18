namespace Prism.ProAssistant.Domain.Tests.Accounting;

using Core;
using Domain.Accounting.Reporting;
using Domain.DayToDay.Appointments;
using FluentAssertions;

public class AccountingReportingPeriodProjectionTests
{
    [Fact]
    public void Project_InvalidType()
    {
        // Arrange

        // Act
        var action = new Action(() => AccountingReportingPeriodProjection.Project(42, new List<Appointment>()));

        // Assert
        action.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void Project_Ok()
    {
        // Arrange
        var appointments = new List<Appointment>
        {
            new()
            {
                Id = Identifier.GenerateString(),
                StartDate = new DateTime(2023, 07, 13),
                State = (int)AppointmentState.Done,
                Payment = (int)PaymentTypes.Cash,
                Price = 42
            },
            new()
            {
                Id = Identifier.GenerateString(),
                StartDate = new DateTime(2023, 07, 1),
                State = (int)AppointmentState.Canceled,
                Payment = (int)PaymentTypes.Cash,
                Price = 42
            },
            new()
            {
                Id = Identifier.GenerateString(),
                StartDate = new DateTime(2023, 07, 25),
                State = (int)AppointmentState.Done,
                Payment = (int)PaymentTypes.Wire,
                Price = 42
            }
        };

        // Act
        var reportingPeriod = AccountingReportingPeriodProjection.Project(12, appointments);

        // Assert
        reportingPeriod.Id.Should().Be("2023-07-01-12");
        reportingPeriod.StartDate.Should().Be(new DateTime(2023, 07, 1));
        reportingPeriod.EndDate.Should().Be(new DateTime(2023, 07, 31));
        reportingPeriod.Type.Should().Be(12);
        reportingPeriod.Income.Should().Be(84);
        reportingPeriod.Details.Should().HaveCount(1);
        reportingPeriod.Details.Should().Contain(x => x.Type == "appointment" && x.UnitPrice == 42 && x.Count == 2 && x.SubTotal == 84);
    }

    [Theory]
    [InlineData(1, "2023-07-01-01")]
    [InlineData(12, "2023-07-01-12")]
    [InlineData(52, "2023-07-01-52")]
    public void Project_Ok_Period(int periodType, string expectedId)
    {
        // Act
        var reportingPeriod = AccountingReportingPeriodProjection.Project(periodType, new List<Appointment>()
        {
            new Appointment
            {
                Id = Identifier.GenerateString(),
                StartDate = new DateTime(2023, 7, 21)
            }
        });

        // Assert
        reportingPeriod.Id.Should().Be(expectedId);
    }
}