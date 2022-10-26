// -----------------------------------------------------------------------
//  <copyright file = "TariffMutationTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.ProAssistant.Api.Graph.Meetings;
using Prism.ProAssistant.Api.Tests.Fakes;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Graph.Meetings;

public class MeetingMutationTests
{
    [Fact]
    public async Task UpsertMeeting_Existing()
    {
        // Arrange
        var meeting = new Meeting
        {
            Title = Identifier.GenerateString(),
            Price = 42,
            Id = Identifier.GenerateString()
        };

        var organisationContext = new OrganizationContextFake();

        // Act
        var query = new MeetingMutation();
        var result = await query.UpsertMeetingAsync(meeting, organisationContext, Mock.Of<ILogger<MeetingMutation>>(), Mock.Of<IUserContextAccessor>());

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task UpsertMeeting_New()
    {
        // Arrange
        var meeting = new Meeting
        {
            Price = 42
        };

        var organisationContext = new OrganizationContextFake();

        // Act
        var query = new MeetingMutation();
        var result = await query.UpsertMeetingAsync(meeting, organisationContext, Mock.Of<ILogger<MeetingMutation>>(), Mock.Of<IUserContextAccessor>());

        // Assert
        result.Should().NotBeNull();
    }
}