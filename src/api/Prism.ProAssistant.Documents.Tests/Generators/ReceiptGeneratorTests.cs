// -----------------------------------------------------------------------
//  <copyright file = "ReceiptGeneratorTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Queries;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Documents.Generators;
using Prism.ProAssistant.Documents.Locales;
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

        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<FindOne<Meeting>>(), CancellationToken.None))
            .ReturnsAsync(meeting);

        mediator.Setup(x => x.Send(It.Is<FindOne<Setting>>(s => s.Id == "documents-headers"), CancellationToken.None))
            .ReturnsAsync(new Setting
            {
                Id = "documents-headers",
                Value =
                    "{\"name\":\"Baudart Simon - PRISM\",\"address\":\"Vieux Chemin de Lille 25B\\n7501 Orcq\\nTVA : BE692.946.818\",\"logo\":\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEoAAAAlCAIAAABqEOipAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABDSURBVGhD7c8BDQAgDMAw7F0nF4qOkSY10HNnP6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplem1zX7ANq7txGhH62zAAAAAElFTkSuQmCC\",\"yourName\":\"Simon Baudart\",\"yourCity\":\"Orcq\",\"signature\":\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEoAAAAlCAIAAABqEOipAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABDSURBVGhD7c8BDQAgDMAw7F0nF4qOkSY10HNnP6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplem1zX7ANq7txGhH62zAAAAAElFTkSuQmCC\"}"
            });

        var localizer = new Mock<ILocalizator>();
        localizer.Setup(x => x.Locale).Returns("fr");

        // Act
        var generator = new ReceiptGenerator(mediator.Object, localizer.Object, Mock.Of<ILogger<ReceiptGenerator>>());
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
            Id = id,
        };

        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<FindOne<Meeting>>(), CancellationToken.None))
            .ReturnsAsync(meeting);

        mediator.Setup(x => x.Send(It.Is<FindOne<Setting>>(s => s.Id == "documents-headers"), CancellationToken.None))
            .ReturnsAsync(new Setting
            {
                Id = "documents-headers",
                Value =
                    "{\"name\":\"Baudart Simon - PRISM\",\"address\":\"Vieux Chemin de Lille 25B\\n7501 Orcq\\nTVA : BE692.946.818\",\"logo\":\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEoAAAAlCAIAAABqEOipAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABDSURBVGhD7c8BDQAgDMAw7F0nF4qOkSY10HNnP6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplem1zX7ANq7txGhH62zAAAAAElFTkSuQmCC\",\"yourName\":\"Simon Baudart\",\"yourCity\":\"Orcq\",\"signature\":\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEoAAAAlCAIAAABqEOipAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABDSURBVGhD7c8BDQAgDMAw7F0nF4qOkSY10HNnP6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplem1zX7ANq7txGhH62zAAAAAElFTkSuQmCC\"}"
            });

        var localizer = new Mock<ILocalizator>();
        localizer.Setup(x => x.Locale).Returns("fr");

        // Act
        var generator = new ReceiptGenerator(mediator.Object, localizer.Object, Mock.Of<ILogger<ReceiptGenerator>>());
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
            Id = id,
            PatientId = Identifier.GenerateString()
        };

        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<FindOne<Meeting>>(), CancellationToken.None))
            .ReturnsAsync(meeting);

        mediator.Setup(x => x.Send(It.IsAny<FindOne<Patient>>(), CancellationToken.None))
            .ReturnsAsync(new Patient
            {
                Id = meeting.PatientId
            });

        var localizer = new Mock<ILocalizator>();
        localizer.Setup(x => x.Locale).Returns("fr");

        // Act
        var generator = new ReceiptGenerator(mediator.Object, localizer.Object, Mock.Of<ILogger<ReceiptGenerator>>());
        var receipt = await generator.Generate(id);

        // Assert
        receipt.Should().BeNull();
    }

    [Fact]
    public async Task Generate_NotFound()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var mediator = new Mock<IMediator>();
        var localizer = new Mock<ILocalizator>();

        // Act
        var generator = new ReceiptGenerator(mediator.Object, localizer.Object, Mock.Of<ILogger<ReceiptGenerator>>());
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

        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<FindOne<Meeting>>(), CancellationToken.None))
            .ReturnsAsync(meeting);

        mediator.Setup(x => x.Send(It.Is<FindOne<Setting>>(s => s.Id == "documents-headers"), CancellationToken.None))
            .ReturnsAsync(new Setting
            {
                Id = "documents-headers",
                Value =
                    "{\"name\":\"Baudart Simon - PRISM\",\"address\":\"Vieux Chemin de Lille 25B\\n7501 Orcq\\nTVA : BE692.946.818\",\"logo\":\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEoAAAAlCAIAAABqEOipAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABDSURBVGhD7c8BDQAgDMAw7F0nF4qOkSY10HNnP6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplem1zX7ANq7txGhH62zAAAAAElFTkSuQmCC\",\"yourName\":\"Simon Baudart\",\"yourCity\":\"Orcq\",\"signature\":\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEoAAAAlCAIAAABqEOipAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABDSURBVGhD7c8BDQAgDMAw7F0nF4qOkSY10HNnP6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplem1zX7ANq7txGhH62zAAAAAElFTkSuQmCC\"}"
            });

        mediator.Setup(x => x.Send(It.IsAny<FindOne<Patient>>(), CancellationToken.None))
            .ReturnsAsync(new Patient
            {
                Id = meeting.PatientId
            });

        var localizer = new Mock<ILocalizator>();
        localizer.Setup(x => x.Locale).Returns("fr");

        // Act
        var generator = new ReceiptGenerator(mediator.Object, localizer.Object, Mock.Of<ILogger<ReceiptGenerator>>());
        var receipt = await generator.Generate(id);

        // Assert
        receipt.Should().NotBeNull();
    }
}