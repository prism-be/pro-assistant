// -----------------------------------------------------------------------
//  <copyright file = "SerializationTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json;
using FluentAssertions;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;

namespace Prism.ProAssistant.Business.Tests;

public class SerializationTests
{
    private readonly Random _dice = new();

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
    public void Meeting_Ok()
    {
        // Arrange
        var meeting = new Meeting
        {
            Id = Identifier.GenerateString(),
            Duration = _dice.Next(0, 42),
            Payment = (int)PaymentTypes.Wire,
            Price = _dice.Next(0, 42),
            State = (int)MeetingState.Confirmed,
            PaymentDate = DateTime.UtcNow,
            Title = Identifier.GenerateString(),
            PatientId = Identifier.GenerateString(),
            StartDate = DateTime.UtcNow.AddHours(-2)
        };

        // Act and Assert
        CheckSerialization(meeting);
    }

    [Fact]
    public void Patient_Ok()
    {
        // Arrange
        var patient = new Patient
        {
            Id = Identifier.GenerateString(),
            BirthDate = Identifier.GenerateString(),
            City = Identifier.GenerateString(),
            Country = Identifier.GenerateString(),
            Number = Identifier.GenerateString(),
            Street = Identifier.GenerateString(),
            FirstName = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            ZipCode = Identifier.GenerateString()
        };

        // Act and Assert
        CheckSerialization(patient);
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
            DefaultDuration = _dice.Next(0, 42)
        };

        // Act and Assert
        CheckSerialization(organization);
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