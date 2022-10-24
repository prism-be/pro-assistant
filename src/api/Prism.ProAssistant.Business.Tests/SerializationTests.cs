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
    public void Organization_Ok()
    {
        // Arrange
        var organization = new Organization
        {
            Id = Identifier.Generate()
        };

        // Act and Assert
        CheckSerialization(organization);
    }

    [Fact]
    public void Patient_Ok()
    {
        // Arrange
        var organization = new Patient
        {
            Id = Identifier.Generate(),
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
        CheckSerialization(organization);
    }

    [Fact]
    public void Tarif_Ok()
    {
        // Arrange
        var organization = new Tarif
        {
            Id = Identifier.Generate(),
            Name = Identifier.GenerateString(),
            Price = (decimal)_dice.NextDouble()
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