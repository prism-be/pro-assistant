// -----------------------------------------------------------------------
//  <copyright file = "MeetingTypeTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using HotChocolate.Types;
using Moq;
using Prism.ProAssistant.Api.Graph.Meetings;
using Prism.ProAssistant.Business.Models;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Graph.Meetings;

public class MeetingTypeTests
{
    [Fact]
    public void Configure_Ok()
    {
        // Arrange
        var descriptor = new Mock<IObjectTypeDescriptor<Meeting>>();

        // Act
        MeetingType.ConfigureMeeting(descriptor.Object);

        // Assert
        descriptor.Invocations.Count.Should().Be(9);
    }
}