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
    public async Task Generate_NoContact()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var documentId = Identifier.GenerateString();
        var appointment = new Appointment
        {
            Id = id,
            ContactId = Identifier.GenerateString(),
            FirstName = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            Title = Identifier.GenerateString()
        };

        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<FindOne<Appointment>>(), CancellationToken.None))
            .ReturnsAsync(appointment);

        SetupSettings(mediator);

        var localizer = new Mock<ILocalizator>();
        localizer.Setup(x => x.Locale).Returns("fr");

        var organizationContext = new Mock<IOrganizationContext>();
        organizationContext.SetupCollection<Appointment>();

        // Act and assert
        var generator = new GenerateDocumentHandler(mediator.Object, Mock.Of<ILogger<GenerateDocumentHandler>>(), localizer.Object, organizationContext.Object);
        await Assert.ThrowsAsync<NotSupportedException>(async () => await generator.Handle(new GenerateDocument(documentId, id), CancellationToken.None));
    }

    [Fact]
    public async Task Generate_NoContactId()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var documentId = Identifier.GenerateString();

        var appointment = new Appointment
        {
            Id = id,
            ContactId = Identifier.GenerateString(),
            FirstName = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            Title = Identifier.GenerateString()
        };

        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<FindOne<Appointment>>(), CancellationToken.None))
            .ReturnsAsync(appointment);

        SetupSettings(mediator);

        var localizer = new Mock<ILocalizator>();
        localizer.Setup(x => x.Locale).Returns("fr");

        var organizationContext = new Mock<IOrganizationContext>();
        organizationContext.SetupCollection<Appointment>();

        // Act and assert
        var generator = new GenerateDocumentHandler(mediator.Object, Mock.Of<ILogger<GenerateDocumentHandler>>(), localizer.Object, organizationContext.Object);
        await Assert.ThrowsAsync<NotSupportedException>(async () => await generator.Handle(new GenerateDocument(documentId, id), CancellationToken.None));
    }

    [Fact]
    public async Task Generate_NoDocument()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var documentId = Identifier.GenerateString();

        var appointment = new Appointment
        {
            Id = id,
            ContactId = Identifier.GenerateString(),
            FirstName = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            Title = Identifier.GenerateString()
        };

        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<FindOne<Appointment>>(), CancellationToken.None))
            .ReturnsAsync(appointment);

        SetupSettings(mediator);

        mediator.Setup(x => x.Send(It.IsAny<FindOne<Contact>>(), CancellationToken.None))
            .ReturnsAsync(new Contact
            {
                Id = appointment.ContactId
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
            ContactId = Identifier.GenerateString(),
            FirstName = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            Title = Identifier.GenerateString()
        };

        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<FindOne<Appointment>>(), CancellationToken.None))
            .ReturnsAsync(appointment);

        SetupSettings(mediator);

        mediator.Setup(x => x.Send(It.IsAny<FindOne<Contact>>(), CancellationToken.None))
            .ReturnsAsync(new Contact
            {
                Id = appointment.ContactId
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
            Documents = new List<BinaryDocument>(),
            FirstName = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            Title = Identifier.GenerateString()
        });
        var bucket = organizationContext.SetupBucket();

        // Act
        var generator = new GenerateDocumentHandler(mediator.Object, Mock.Of<ILogger<GenerateDocumentHandler>>(), localizer.Object, organizationContext.Object);
        var receipt = await generator.Handle(new GenerateDocument(documentId, id), CancellationToken.None);

        // Assert
        receipt.Should().NotBeNull();
        bucket.Verify(x => x.UploadFromBytesAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<GridFSUploadOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private static void SetupSettings(Mock<IMediator> mediator)
    {
        mediator.Setup(x => x.Send(It.IsAny<FindMany<Setting>>(), CancellationToken.None))
            .ReturnsAsync(new List<Setting>
            {
                new()
                {
                    Id = "document-header-name",
                    Value = "Simon Baudart"
                },
                new()
                {
                    Id = "document-header-address",
                    Value = "Vieux Chemin de Lille 25B\\n7501 Orcq\\nTVA : BE692.946.818"
                },
                new()
                {
                    Id = "document-header-logo",
                    Value =
                        "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEoAAAAlCAIAAABqEOipAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABDSURBVGhD7c8BDQAgDMAw7F0nF4qOkSY10HNnP6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplem1zX7ANq7txGhH62zAAAAAElFTkSuQmCC"
                },
                new()
                {
                    Id = "document-header-signature",
                    Value =
                        "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEoAAAAlCAIAAABqEOipAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABDSURBVGhD7c8BDQAgDMAw7F0nF4qOkSY10HNnP6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplem1zX7ANq7txGhH62zAAAAAElFTkSuQmCC"
                },
                new()
                {
                    Id = "document-header-your-name",
                    Value = "Simon Baudart"
                },
                new()
                {
                    Id = "document-header-your-city",
                    Value = "Orcq"
                },
                new()
                {
                    Id = "document-header-accentuate-color",
                    Value = "#123456"
                }
            });
    }
}