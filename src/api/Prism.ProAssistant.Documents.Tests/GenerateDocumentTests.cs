// -----------------------------------------------------------------------
//  <copyright file = "GenerateDocumentTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver.GridFS;
using Moq;
using Prism.ProAssistant.Business.Exceptions;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Queries;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;
using Prism.ProAssistant.Documents.Locales;
using Prism.ProAssistant.UnitTesting;
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

        var appointment = new Appointment
        {
            Id = id,
            PatientId = Identifier.GenerateString()
        };

        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<FindOne<Appointment>>(), CancellationToken.None))
            .ReturnsAsync(appointment);

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
                Id = appointment.PatientId
            });

        var localizer = new Mock<ILocalizator>();
        localizer.Setup(x => x.Locale).Returns("fr");

        var organizationContext = new Mock<IOrganizationContext>();
        organizationContext.SetupCollection<Appointment>();

        // Act and assert
        var generator = new GenerateDocumentHandler(mediator.Object, Mock.Of<ILogger<GenerateDocumentHandler>>(), localizer.Object, organizationContext.Object);
        await Assert.ThrowsAsync<NotFoundException>(async () => await generator.Handle(new GenerateDocument(documentId, id), CancellationToken.None));
    }

    [Fact]
    public async Task Generate_NoPatient()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var documentId = Identifier.GenerateString();
        var appointment = new Appointment
        {
            Id = id,
            PatientId = Identifier.GenerateString()
        };

        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<FindOne<Appointment>>(), CancellationToken.None))
            .ReturnsAsync(appointment);

        mediator.Setup(x => x.Send(It.Is<FindOne<Setting>>(s => s.Id == "documents-headers"), CancellationToken.None))
            .ReturnsAsync(new Setting
            {
                Id = "documents-headers",
                Value =
                    "{\"name\":\"Baudart Simon - PRISM\",\"address\":\"Vieux Chemin de Lille 25B\\n7501 Orcq\\nTVA : BE692.946.818\",\"logo\":\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEoAAAAlCAIAAABqEOipAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABDSURBVGhD7c8BDQAgDMAw7F0nF4qOkSY10HNnP6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplem1zX7ANq7txGhH62zAAAAAElFTkSuQmCC\",\"yourName\":\"Simon Baudart\",\"yourCity\":\"Orcq\",\"signature\":\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEoAAAAlCAIAAABqEOipAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABDSURBVGhD7c8BDQAgDMAw7F0nF4qOkSY10HNnP6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplem1zX7ANq7txGhH62zAAAAAElFTkSuQmCC\"}"
            });

        var localizer = new Mock<ILocalizator>();
        localizer.Setup(x => x.Locale).Returns("fr");

        var organizationContext = new Mock<IOrganizationContext>();
        organizationContext.SetupCollection<Appointment>();

        // Act and assert
        var generator = new GenerateDocumentHandler(mediator.Object, Mock.Of<ILogger<GenerateDocumentHandler>>(), localizer.Object, organizationContext.Object);
        await Assert.ThrowsAsync<NotSupportedException>(async () => await generator.Handle(new GenerateDocument(documentId, id), CancellationToken.None));
    }

    [Fact]
    public async Task Generate_NoPatientId()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var documentId = Identifier.GenerateString();

        var appointment = new Appointment
        {
            Id = id
        };

        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<FindOne<Appointment>>(), CancellationToken.None))
            .ReturnsAsync(appointment);

        mediator.Setup(x => x.Send(It.Is<FindOne<Setting>>(s => s.Id == "documents-headers"), CancellationToken.None))
            .ReturnsAsync(new Setting
            {
                Id = "documents-headers",
                Value =
                    "{\"name\":\"Baudart Simon - PRISM\",\"address\":\"Vieux Chemin de Lille 25B\\n7501 Orcq\\nTVA : BE692.946.818\",\"logo\":\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEoAAAAlCAIAAABqEOipAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABDSURBVGhD7c8BDQAgDMAw7F0nF4qOkSY10HNnP6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplem1zX7ANq7txGhH62zAAAAAElFTkSuQmCC\",\"yourName\":\"Simon Baudart\",\"yourCity\":\"Orcq\",\"signature\":\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEoAAAAlCAIAAABqEOipAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABDSURBVGhD7c8BDQAgDMAw7F0nF4qOkSY10HNnP6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplem1zX7ANq7txGhH62zAAAAAElFTkSuQmCC\"}"
            });

        var localizer = new Mock<ILocalizator>();
        localizer.Setup(x => x.Locale).Returns("fr");

        var organizationContext = new Mock<IOrganizationContext>();
        organizationContext.SetupCollection<Appointment>();

        // Act and assert
        var generator = new GenerateDocumentHandler(mediator.Object, Mock.Of<ILogger<GenerateDocumentHandler>>(), localizer.Object, organizationContext.Object);
        await Assert.ThrowsAsync<NotSupportedException>(async () => await generator.Handle(new GenerateDocument(documentId, id), CancellationToken.None));
    }

    [Fact]
    public async Task Generate_NoSettings()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var documentId = Identifier.GenerateString();
        var appointment = new Appointment
        {
            Id = id,
            PatientId = Identifier.GenerateString()
        };

        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<FindOne<Appointment>>(), CancellationToken.None))
            .ReturnsAsync(appointment);

        mediator.Setup(x => x.Send(It.IsAny<FindOne<Patient>>(), CancellationToken.None))
            .ReturnsAsync(new Patient
            {
                Id = appointment.PatientId
            });

        var localizer = new Mock<ILocalizator>();
        localizer.Setup(x => x.Locale).Returns("fr");

        var organizationContext = new Mock<IOrganizationContext>();
        organizationContext.SetupCollection<Appointment>();

        // Act and assert
        var generator = new GenerateDocumentHandler(mediator.Object, Mock.Of<ILogger<GenerateDocumentHandler>>(), localizer.Object, organizationContext.Object);
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

        var organizationContext = new Mock<IOrganizationContext>();
        organizationContext.SetupCollection<Appointment>();

        // Act and assert
        var generator = new GenerateDocumentHandler(mediator.Object, Mock.Of<ILogger<GenerateDocumentHandler>>(), localizer.Object, organizationContext.Object);
        await Assert.ThrowsAsync<NotSupportedException>(async () => await generator.Handle(new GenerateDocument(documentId, id), CancellationToken.None));
    }

    [Fact]
    public async Task Generate_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var documentId = Identifier.GenerateString();

        var appointment = new Appointment
        {
            Id = id,
            PatientId = Identifier.GenerateString()
        };

        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<FindOne<Appointment>>(), CancellationToken.None))
            .ReturnsAsync(appointment);

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
                Id = appointment.PatientId
            });

        mediator.Setup(x => x.Send(It.Is<FindOne<DocumentConfiguration>>(d => d.Id == documentId), CancellationToken.None))
            .ReturnsAsync(new DocumentConfiguration
            {
                Id = documentId,
                Body = Identifier.GenerateString(),
                Title = Identifier.GenerateString(),
                Name = Identifier.GenerateString()
            });

        var localizer = new Mock<ILocalizator>();
        localizer.Setup(x => x.Locale).Returns("fr");

        var organizationContext = new Mock<IOrganizationContext>();

        organizationContext.SetupCollection(new Appointment
        {
            Id = id,
            Documents = new List<BinaryDocument>()
        });
        var bucket = organizationContext.SetupBucket();

        // Act
        var generator = new GenerateDocumentHandler(mediator.Object, Mock.Of<ILogger<GenerateDocumentHandler>>(), localizer.Object, organizationContext.Object);
        var receipt = await generator.Handle(new GenerateDocument(documentId, id), CancellationToken.None);

        // Assert
        receipt.Should().NotBeNull();
        bucket.Verify(x => x.UploadFromBytesAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<GridFSUploadOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}