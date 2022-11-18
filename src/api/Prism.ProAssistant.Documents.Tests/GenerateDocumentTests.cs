﻿// -----------------------------------------------------------------------
//  <copyright file = "GenerateDocumentTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.ProAssistant.Business.Exceptions;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Queries;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Documents.Locales;
using Xunit;

namespace Prism.ProAssistant.Documents.Tests;

public class GenerateDocumentTests
{

    [Fact]
    public async Task Generate_NoDocument()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var documentId = Identifier.GenerateString();

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

        // Act and assert
        var generator = new GenerateDocumentHandler(mediator.Object, Mock.Of<ILogger<GenerateDocumentHandler>>(), localizer.Object);
        await Assert.ThrowsAsync<NotFoundException>(async () => await generator.Handle(new GenerateDocument(documentId, id), CancellationToken.None));
    }

    [Fact]
    public async Task Generate_NoPatient()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var documentId = Identifier.GenerateString();
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

        // Act and assert
        var generator = new GenerateDocumentHandler(mediator.Object, Mock.Of<ILogger<GenerateDocumentHandler>>(), localizer.Object);
        await Assert.ThrowsAsync<NotSupportedException>(async () => await generator.Handle(new GenerateDocument(documentId, id), CancellationToken.None));
    }

    [Fact]
    public async Task Generate_NoPatientId()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var documentId = Identifier.GenerateString();

        var meeting = new Meeting
        {
            Id = id
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

        // Act and assert
        var generator = new GenerateDocumentHandler(mediator.Object, Mock.Of<ILogger<GenerateDocumentHandler>>(), localizer.Object);
        await Assert.ThrowsAsync<NotSupportedException>(async () => await generator.Handle(new GenerateDocument(documentId, id), CancellationToken.None));
    }

    [Fact]
    public async Task Generate_NoSettings()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var documentId = Identifier.GenerateString();
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

        // Act and assert
        var generator = new GenerateDocumentHandler(mediator.Object, Mock.Of<ILogger<GenerateDocumentHandler>>(), localizer.Object);
        await Assert.ThrowsAsync<NotSupportedException>(async () => await generator.Handle(new GenerateDocument(documentId, id), CancellationToken.None));
    }

    [Fact]
    public async Task Generate_NotFound()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var documentId = Identifier.GenerateString();

        var mediator = new Mock<IMediator>();
        var localizer = new Mock<ILocalizator>();

        // Act and assert
        var generator = new GenerateDocumentHandler(mediator.Object, Mock.Of<ILogger<GenerateDocumentHandler>>(), localizer.Object);
        await Assert.ThrowsAsync<NotSupportedException>(async () => await generator.Handle(new GenerateDocument(documentId, id), CancellationToken.None));
    }

    [Fact]
    public async Task Generate_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var documentId = Identifier.GenerateString();

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

        mediator.Setup(x => x.Send(It.Is<FindOne<Document>>(d => d.Id == documentId), CancellationToken.None))
            .ReturnsAsync(new Document
            {
                Id = documentId,
                Body = Identifier.GenerateString(),
                Title = Identifier.GenerateString(),
                Name = Identifier.GenerateString()
            });

        var localizer = new Mock<ILocalizator>();
        localizer.Setup(x => x.Locale).Returns("fr");

        // Act
        var generator = new GenerateDocumentHandler(mediator.Object, Mock.Of<ILogger<GenerateDocumentHandler>>(), localizer.Object);
        var receipt = await generator.Handle(new GenerateDocument(documentId, id), CancellationToken.None);

        // Assert
        receipt.Should().NotBeNull();
    }
}