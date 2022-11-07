// -----------------------------------------------------------------------
//  <copyright file = "MeetingQueryTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Prism.ProAssistant.Api.Graph.Meetings;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.UnitTesting.Fakes;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Graph.Meetings;

public class MeetingQueryTests
{
    [Fact]
    public async Task GetMeetingById_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();

        var existingMeeting = new Meeting
        {
            Id = id,
            Title = Identifier.GenerateString()
        };

        var organisationContext = new OrganizationContextFake();
        await organisationContext.Meetings.InsertOneAsync(existingMeeting);

        // Act
        var query = new MeetingQuery();
        var result = query.GetMeetingById(id, organisationContext);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetMeetings_Ok()
    {
        // Arrange
        var organisationContext = new OrganizationContextFake();

        await organisationContext.Meetings.InsertOneAsync(new Meeting
        {
            Id = Identifier.GenerateString(),
            Title = Identifier.GenerateString(),
            StartDate = DateTime.UtcNow
        });

        await organisationContext.Meetings.InsertOneAsync(new Meeting
        {
            Id = Identifier.GenerateString(),
            Title = Identifier.GenerateString(),
            StartDate = DateTime.UtcNow
        });
        await organisationContext.Meetings.InsertOneAsync(new Meeting
        {
            Id = Identifier.GenerateString(),
            Title = Identifier.GenerateString(),
            StartDate = DateTime.UtcNow
        });

        // Act
        var query = new MeetingQuery();
        var result = query.GetMeetings(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), organisationContext);

        // Assert
        result.Should().NotBeNull();
    }
}