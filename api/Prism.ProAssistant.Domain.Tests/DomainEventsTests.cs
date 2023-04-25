using System.Text.Json;
using FluentAssertions;
using Prism.Core;

namespace Prism.ProAssistant.Domain.Tests;

public class DomainEventsTests
{
    [Fact]
    public void Serialize_Ok()
    {
        // Arrange
        var e = new DomainEvent
        {
            Data = Identifier.GenerateString(),
            Id = Identifier.GenerateString(),
            StreamId = Identifier.GenerateString(),
            StreamType = Identifier.GenerateString(),
            Type = Identifier.GenerateString(),
            UserId = Identifier.GenerateString(),
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var json = JsonSerializer.Serialize(e);
        var e2 = JsonSerializer.Deserialize<DomainEvent>(json);

        // Assert
        e2.Should().BeEquivalentTo(e);
    }
}