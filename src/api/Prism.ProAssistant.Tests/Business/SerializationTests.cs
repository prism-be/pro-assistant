using System.Text.Json;
using FluentAssertions;
using Prism.ProAssistant.Business.Users;

namespace Prism.ProAssistant.Tests.Business;

public class SerializationTests
{
    [Fact]
    public void User_Ok()
    {
        var user = new User(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        CheckSerialization(user);
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