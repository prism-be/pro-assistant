// -----------------------------------------------------------------------
//  <copyright file = "OrganizationContextEnricherTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Moq;
using Prism.ProAssistant.Api.Insights;
using Prism.ProAssistant.Business.Security;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Insights;

public class OrganizationContextEnricherTests
{
    [Fact]
    public void Enrigh_Ok()
    {
        // Arrange
        var userContextAccessor = new Mock<IUserContextAccessor>();
        userContextAccessor.Setup(x => x.UserId).Returns(Identifier.GenerateString);
        userContextAccessor.Setup(x => x.OrganizationId).Returns(Identifier.GenerateString);

        var logEventFactory = new Mock<ILogEventPropertyFactory>();
        logEventFactory.Setup(x => x.CreateProperty("UserId", It.IsAny<object?>(), false)).Returns(new LogEventProperty("UserId", new ScalarValue("UserId")));
        logEventFactory.Setup(x => x.CreateProperty("OrganizationId", It.IsAny<object?>(), false)).Returns(new LogEventProperty("OrganizationId", new ScalarValue("OrganizationId")));

        // Act
        var logEvent = new LogEvent(DateTimeOffset.Now, LogEventLevel.Debug, null, new MessageTemplate("test", new List<MessageTemplateToken>()), new List<LogEventProperty>());
        var enricher = new OrganizationContextEnricher(userContextAccessor.Object);
        enricher.Enrich(logEvent, logEventFactory.Object);

        // Assert
        logEvent.Properties.Should().ContainSingle(x => x.Key == "UserId");
        logEvent.Properties.Should().ContainSingle(x => x.Key == "OrganizationId");
    }
}