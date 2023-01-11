// -----------------------------------------------------------------------
//  <copyright file = "SerializationTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json;
using FluentAssertions;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Business.Security;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Models;

public class SerializationTests
{

    [Fact]
    public void DownloadKey_Ok()
    {
        var model = new DownloadKey(Identifier.GenerateString());
        CheckSerialization(model);
    }

    [Fact]
    public void UserInformation_Ok()
    {
        var model = new UserInformation(Identifier.GenerateString(), Identifier.GenerateString(), true);
        CheckSerialization(model);
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