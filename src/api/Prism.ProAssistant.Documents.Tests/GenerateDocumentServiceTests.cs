// -----------------------------------------------------------------------
//  <copyright file = "GenerateDocumentServiceTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver.GridFS;
using Moq;
using Prism.ProAssistant.Business.Exceptions;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Services;
using Prism.ProAssistant.Business.Storage;
using Prism.ProAssistant.Documents.Locales;
using Prism.ProAssistant.UnitTesting;
using Xunit;

namespace Prism.ProAssistant.Documents.Tests;

public class GenerateDocumentServiceTests
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

        var findOneService = new Mock<FindOneService>();
        findOneService.Setup(x => x.Find<Appointment>(It.IsAny<string>())).ReturnsAsync(appointment);

        var findManyService = SetupSettings();

        var localizer = new Mock<ILocalizator>();
        localizer.Setup(x => x.Locale).Returns("fr");

        var organizationContext = new Mock<IOrganizationContext>();
        organizationContext.SetupCollection<Appointment>();

        // Act and assert
        var generator = new GenerateDocumentService(Mock.Of<ILogger<GenerateDocumentService>>(), localizer.Object, organizationContext.Object, findOneService.Object, findManyService.Object);
        await Assert.ThrowsAsync<NotSupportedException>(async () => await generator.Generate(documentId, id));
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

        var findOneService = new Mock<FindOneService>();
        findOneService.Setup(x => x.Find<Appointment>(It.IsAny<string>())).ReturnsAsync(appointment);

        var findManyService = SetupSettings();

        var localizer = new Mock<ILocalizator>();
        localizer.Setup(x => x.Locale).Returns("fr");

        var organizationContext = new Mock<IOrganizationContext>();
        organizationContext.SetupCollection<Appointment>();

        // Act and assert
        var generator = new GenerateDocumentService(Mock.Of<ILogger<GenerateDocumentService>>(), localizer.Object, organizationContext.Object, findOneService.Object, findManyService.Object);
        await Assert.ThrowsAsync<NotSupportedException>(async () => await generator.Generate(documentId, id));
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

        var findOneService = new Mock<FindOneService>();
        findOneService.Setup(x => x.Find<Appointment>(It.IsAny<string>())).ReturnsAsync(appointment);
        findOneService.Setup(x => x.Find<Contact>(It.IsAny<string>())).ReturnsAsync(new Contact
        {
            Id = appointment.ContactId
        });

        var findManyService = SetupSettings();

        var localizer = new Mock<ILocalizator>();
        localizer.Setup(x => x.Locale).Returns("fr");

        var organizationContext = new Mock<IOrganizationContext>();
        organizationContext.SetupCollection<Appointment>();

        // Act and assert
        var generator = new GenerateDocumentService(Mock.Of<ILogger<GenerateDocumentService>>(), localizer.Object, organizationContext.Object, findOneService.Object, findManyService.Object);
        await Assert.ThrowsAsync<NotFoundException>(async () => await generator.Generate(documentId, id));
    }

    [Fact]
    public async Task Generate_NotFound()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var documentId = Identifier.GenerateString();

        var findOneService = new Mock<FindOneService>();
        var findManyService = new Mock<FindManyService>();
        var localizer = new Mock<ILocalizator>();

        var organizationContext = new Mock<IOrganizationContext>();
        organizationContext.SetupCollection<Appointment>();

        // Act and assert
        var generator = new GenerateDocumentService(Mock.Of<ILogger<GenerateDocumentService>>(), localizer.Object, organizationContext.Object, findOneService.Object, findManyService.Object);
        await Assert.ThrowsAsync<NotSupportedException>(async () => await generator.Generate(documentId, id));
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

        var findOneService = new Mock<FindOneService>();
        findOneService.Setup(x => x.Find<Appointment>(It.IsAny<string>())).ReturnsAsync(appointment);
        findOneService.Setup(x => x.Find<Contact>(It.IsAny<string>())).ReturnsAsync(new Contact
        {
            Id = appointment.ContactId
        });
        findOneService.Setup(x => x.Find<DocumentConfiguration>(It.IsAny<string>())).ReturnsAsync(new DocumentConfiguration
        {
            Id = documentId,
            Body = Identifier.GenerateString(),
            Title = Identifier.GenerateString(),
            Name = Identifier.GenerateString()
        });

        var findManyService = SetupSettings();

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
        var generator = new GenerateDocumentService(Mock.Of<ILogger<GenerateDocumentService>>(), localizer.Object, organizationContext.Object, findOneService.Object, findManyService.Object);
        var receipt = await generator.Generate(documentId, id);

        // Assert
        receipt.Should().NotBeNull();
        bucket.Verify(x => x.UploadFromBytesAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<GridFSUploadOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private static Mock<IFindManyService> SetupSettings()
    {
        var findMany = new Mock<IFindManyService>();
        findMany.Setup(x => x.Find<Setting>())
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

        return findMany;
    }
}