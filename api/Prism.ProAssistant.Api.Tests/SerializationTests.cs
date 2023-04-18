using System.Text.Json;
using FluentAssertions;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;
using Prism.ProAssistant.Domain.Configuration;
using Prism.ProAssistant.Domain.Configuration.DocumentConfiguration;
using Prism.ProAssistant.Domain.Configuration.Settings;
using Prism.ProAssistant.Domain.Configuration.Tariffs;
using Prism.ProAssistant.Domain.DayToDay.Appointments;
using Prism.ProAssistant.Storage;

namespace Prism.ProAssistant.Api.Tests;

public class SerializationTests
{
    private readonly Random _dice = new();

    [Fact]
    public void Appointment_Ok()
    {
        // Arrange
        var appointment = new Appointment
        {
            Id = Identifier.GenerateString(),
            Duration = _dice.Next(0, 42),
            Payment = (int)PaymentTypes.Wire,
            Price = _dice.Next(0, 42),
            State = (int)AppointmentState.Confirmed,
            PaymentDate = DateTime.UtcNow,
            Title = Identifier.GenerateString(),
            ContactId = Identifier.GenerateString(),
            StartDate = DateTime.UtcNow.AddHours(-2),
            Type = Identifier.GenerateString(),
            BackgroundColor = Identifier.GenerateString(),
            FirstName = Identifier.GenerateString(),
            ForeColor = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            TypeId = Identifier.GenerateString(),
            Documents = new List<BinaryDocument>
            {
                new()
                {
                    Id = Identifier.GenerateString(),
                    Title = Identifier.GenerateString(),
                    Date = DateTime.UtcNow,
                    FileName = Identifier.GenerateString()
                }
            },
            BirthDate = Identifier.GenerateString(),
            PhoneNumber = Identifier.GenerateString()
        };

        // Act and Assert
        CheckSerialization(appointment);
    }

    [Fact]
    public void Configuration_Ok()
    {
        // Arrange
        var source = new Setting
        {
            Id = Identifier.GenerateString(),
            Value = Identifier.GenerateString()
        };

        // Act and Assert
        CheckSerialization(source);
    }

    [Fact]
    public void Contact_Ok()
    {
        // Arrange
        var contact = new Contact
        {
            Id = Identifier.GenerateString(),
            BirthDate = Identifier.GenerateString(),
            City = Identifier.GenerateString(),
            Country = Identifier.GenerateString(),
            Number = Identifier.GenerateString(),
            Street = Identifier.GenerateString(),
            FirstName = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            ZipCode = Identifier.GenerateString(),
            Title = Identifier.GenerateString(),
            Email = Identifier.GenerateString(),
            MobileNumber = Identifier.GenerateString(),
            PhoneNumber = Identifier.GenerateString()
        };

        // Act and Assert
        CheckSerialization(contact);
    }

    [Fact]
    public void Document()
    {
        // Arrange
        var source = new DocumentConfiguration
        {
            Id = Identifier.GenerateString(),
            Name = Identifier.GenerateString(),
            Title = Identifier.GenerateString(),
            Body = Identifier.GenerateString()
        };

        // Act and Assert
        CheckSerialization(source);
    }

    [Fact]
    public void DocumentRequest_Ok()
    {
        // Arrange
        var source = new DocumentRequest
        {
            AppointmentId = Identifier.GenerateString(),
            DocumentId = Identifier.GenerateString()
        };

        // Act and Assert
        CheckSerialization(source);
    }

    [Fact]
    public void DownloadReference_Ok()
    {
        // Arrange
        var source = new DownloadReference
        {
            Id = Identifier.GenerateString()
        };

        // Act and Assert
        CheckSerialization(source);
    }

    [Fact]
    public void OperationResult_FromBool()
    {
        // Act
        var source = OperationResult.From(true);

        // Assert
        source!.Success.Should().BeTrue();
    }

    [Fact]
    public void OperationResult_Ok()
    {
        // Arrange
        var source = new OperationResult
        {
            Success = true
        };

        // Act and Assert
        CheckSerialization(source);
    }

    [Fact]
    public void Tariff_Ok()
    {
        // Arrange
        var organization = new Tariff
        {
            Id = Identifier.GenerateString(),
            Name = Identifier.GenerateString(),
            Price = (decimal)_dice.NextDouble(),
            DefaultDuration = _dice.Next(0, 42),
            BackgroundColor = Identifier.GenerateString(),
            ForeColor = Identifier.GenerateString()
        };

        // Act and Assert
        CheckSerialization(organization);
    }

    [Fact]
    public void UpsertResult_Ok()
    {
        // Arrange
        var source = new UpsertResult(Identifier.GenerateString());

        // Act and Assert
        CheckSerialization(source);
    }

    [Fact]
    public void User_Ok()
    {
        // Arrange
        var source = new UserInformation
        {
            Name = Identifier.GenerateString(),
            IsAuthenticated = true,
            Organization = Identifier.GenerateString()
        };

        // Act and Assert
        CheckSerialization(source);
    }

    private static void CheckSerialization<T>(T source)
    {
        // Act
        var destination = SerializeAndDeserialize(source);

        // Assert
        destination.Should().BeEquivalentTo(source);
    }

    private static T? SerializeAndDeserialize<T>(T source)
    {
        var json = JsonSerializer.Serialize(source);
        return JsonSerializer.Deserialize<T>(json);
    }
}