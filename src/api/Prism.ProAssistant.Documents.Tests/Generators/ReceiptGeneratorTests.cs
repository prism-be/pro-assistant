// -----------------------------------------------------------------------
//  <copyright file = "ReceiptGeneratorTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Documents.Generators;
using Prism.ProAssistant.Documents.Locales;
using Prism.ProAssistant.UnitTesting;
using Prism.ProAssistant.UnitTesting.Fakes;
using Xunit;

namespace Prism.ProAssistant.Documents.Tests.Generators;

public class ReceiptGeneratorTests
{
    [Fact]
    public async Task Generate_NoPatient()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var meeting = new Meeting
        {
            Id = id,
            PatientId = Identifier.GenerateString()
        };

        var organisationContext = new OrganizationContextFake();
        organisationContext.MeetingsMock = organisationContext.Database.SetupCollection(meeting);
        organisationContext.SettingsMock = organisationContext.Database.SetupCollection(new Setting
        {
            Id = "documents-headers",
            Value =
                "{\"name\":\"Baudart Simon - PRISM\",\"address\":\"Vieux Chemin de Lille 25B\\n7501 Orcq\\nTVA : BE692.946.818\",\"logo\":\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEoAAAAlCAIAAABqEOipAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABDSURBVGhD7c8BDQAgDMAw7F0nF4qOkSY10HNnP6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplem1zX7ANq7txGhH62zAAAAAElFTkSuQmCC\",\"yourName\":\"Simon Baudart\",\"yourCity\":\"Orcq\",\"signature\":\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEoAAAAlCAIAAABqEOipAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABDSURBVGhD7c8BDQAgDMAw7F0nF4qOkSY10HNnP6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplem1zX7ANq7txGhH62zAAAAAElFTkSuQmCC\"}"
        });
        var localizer = new Mock<ILocalizator>();
        localizer.Setup(x => x.Locale).Returns("fr");

        // Act
        var generator = new ReceiptGenerator(organisationContext, localizer.Object, Mock.Of<ILogger<ReceiptGenerator>>());
        var receipt = await generator.Generate(id);

        // Assert
        receipt.Should().BeNull();
    }
    
    [Fact]
    public async Task Generate_NoPatientId()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var meeting = new Meeting
        {
            Id = id
        };

        var organisationContext = new OrganizationContextFake();
        organisationContext.MeetingsMock = organisationContext.Database.SetupCollection(meeting);
        organisationContext.SettingsMock = organisationContext.Database.SetupCollection(new Setting
        {
            Id = "documents-headers",
            Value =
                "{\"name\":\"Baudart Simon - PRISM\",\"address\":\"Vieux Chemin de Lille 25B\\n7501 Orcq\\nTVA : BE692.946.818\",\"logo\":\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEoAAAAlCAIAAABqEOipAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABDSURBVGhD7c8BDQAgDMAw7F0nF4qOkSY10HNnP6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplem1zX7ANq7txGhH62zAAAAAElFTkSuQmCC\",\"yourName\":\"Simon Baudart\",\"yourCity\":\"Orcq\",\"signature\":\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEoAAAAlCAIAAABqEOipAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABDSURBVGhD7c8BDQAgDMAw7F0nF4qOkSY10HNnP6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplem1zX7ANq7txGhH62zAAAAAElFTkSuQmCC\"}"
        });
        var localizer = new Mock<ILocalizator>();
        localizer.Setup(x => x.Locale).Returns("fr");

        // Act
        var generator = new ReceiptGenerator(organisationContext, localizer.Object, Mock.Of<ILogger<ReceiptGenerator>>());
        var receipt = await generator.Generate(id);

        // Assert
        receipt.Should().BeNull();
    }

    [Fact]
    public async Task Generate_NoSettings()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var meeting = new Meeting
        {
            Id = id
        };

        var organisationContext = new OrganizationContextFake();
        organisationContext.MeetingsMock = organisationContext.Database.SetupCollection(meeting);

        var localizer = new Mock<ILocalizator>();
        localizer.Setup(x => x.Locale).Returns("fr");

        // Act
        var generator = new ReceiptGenerator(organisationContext, localizer.Object, Mock.Of<ILogger<ReceiptGenerator>>());
        var receipt = await generator.Generate(id);

        // Assert
        receipt.Should().BeNull();
    }

    [Fact]
    public async Task Generate_NotFound()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var organisationContext = new OrganizationContextFake();
        var localizer = new Mock<ILocalizator>();

        // Act
        var generator = new ReceiptGenerator(organisationContext, localizer.Object, Mock.Of<ILogger<ReceiptGenerator>>());
        var receipt = await generator.Generate(id);

        // Assert
        receipt.Should().BeNull();
    }

    [Fact]
    public async Task Generate_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var meeting = new Meeting
        {
            Id = id,
            PatientId = Identifier.GenerateString()
        };

        var organisationContext = new OrganizationContextFake();
        organisationContext.MeetingsMock = organisationContext.Database.SetupCollection(meeting);
        organisationContext.SettingsMock = organisationContext.Database.SetupCollection(new Setting
        {
            Id = "documents-headers",
            Value =
                "{\"name\":\"Baudart Simon - PRISM\",\"address\":\"Vieux Chemin de Lille 25B\\n7501 Orcq\\nTVA : BE692.946.818\",\"logo\":\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEoAAAAlCAIAAABqEOipAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABDSURBVGhD7c8BDQAgDMAw7F0nF4qOkSY10HNnP6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplem1zX7ANq7txGhH62zAAAAAElFTkSuQmCC\",\"yourName\":\"Simon Baudart\",\"yourCity\":\"Orcq\",\"signature\":\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEoAAAAlCAIAAABqEOipAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABDSURBVGhD7c8BDQAgDMAw7F0nF4qOkSY10HNnP6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplem1zX7ANq7txGhH62zAAAAAElFTkSuQmCC\"}"
        });
        organisationContext.PatientsMock = organisationContext.Database.SetupCollection(new Patient
        {
            Id = meeting.PatientId
        });
        var localizer = new Mock<ILocalizator>();
        localizer.Setup(x => x.Locale).Returns("fr");

        // Act
        var generator = new ReceiptGenerator(organisationContext, localizer.Object, Mock.Of<ILogger<ReceiptGenerator>>());
        var receipt = await generator.Generate(id);

        // Assert
        receipt.Should().NotBeNull();
    }
}