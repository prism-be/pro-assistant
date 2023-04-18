using Microsoft.Extensions.Logging;
using Moq;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;
using Prism.ProAssistant.Domain.Configuration;
using Prism.ProAssistant.Domain.Configuration.DocumentConfiguration;
using Prism.ProAssistant.Domain.Configuration.Settings;
using Prism.ProAssistant.Domain.DayToDay.Appointments;
using Prism.ProAssistant.Storage;

namespace Prism.ProAssistant.Api.Tests.Services;

public class PdfServiceTests
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
            FirstName = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            Title = Identifier.GenerateString()
        };

        var dataService = new Mock<IQueryService>();
        dataService.Setup(x => x.SingleAsync<Appointment>(It.IsAny<string>())).ReturnsAsync(appointment);
        dataService.Setup(x => x.SingleAsync<DocumentConfiguration>(It.IsAny<string>())).ReturnsAsync(new DocumentConfiguration
        {
            Id = documentId,
            Body = Identifier.GenerateString(),
            Title = Identifier.GenerateString(),
            Name = Identifier.GenerateString()
        });

        SetupSettings(dataService);

        var eventService = new Mock<IEventService>();
        var fileService = new Mock<IFileService>();

        // Act
        var service = new PdfService(dataService.Object, Mock.Of<ILogger<PdfService>>(), eventService.Object, fileService.Object);
        await service.GenerateDocument(new DocumentRequest
            { AppointmentId = id, DocumentId = documentId });

        // Assert
        fileService.Verify(x => x.UploadFromBytesAsync(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Once);
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

        var dataService = new Mock<IQueryService>();
        dataService.Setup(x => x.SingleAsync<Appointment>(It.IsAny<string>())).ReturnsAsync(appointment);
        dataService.Setup(x => x.SingleAsync<Contact>(It.IsAny<string>())).ReturnsAsync(new Contact
        {
            Id = appointment.ContactId
        });
        dataService.Setup(x => x.SingleAsync<DocumentConfiguration>(It.IsAny<string>())).ReturnsAsync(new DocumentConfiguration
        {
            Id = documentId,
            Body = Identifier.GenerateString(),
            Title = Identifier.GenerateString(),
            Name = Identifier.GenerateString()
        });

        SetupSettings(dataService);

        var eventService = new Mock<IEventService>();
        var fileService = new Mock<IFileService>();

        // Act
        var service = new PdfService(dataService.Object, Mock.Of<ILogger<PdfService>>(), eventService.Object, fileService.Object);
        await service.GenerateDocument(new DocumentRequest
            { AppointmentId = id, DocumentId = documentId });

        // Assert
        fileService.Verify(x => x.UploadFromBytesAsync(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Once);
    }

    private static void SetupSettings(Mock<IQueryService> dataService)
    {
        dataService.Setup(x => x.SingleAsync<Setting>("document-header-name")).ReturnsAsync(new Setting
        {
            Id = "document-header-name",
            Value = "Simon Baudart"
        });

        dataService.Setup(x => x.SingleAsync<Setting>("document-header-address")).ReturnsAsync(new Setting
        {
            Id = "document-header-address",
            Value = "Vieux Chemin de Lille 25B\\n7501 Orcq\\nTVA : BE692.946.818"
        });

        dataService.Setup(x => x.SingleAsync<Setting>("document-header-logo")).ReturnsAsync(new Setting
        {
            Id = "document-header-logo",
            Value =
                "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEoAAAAlCAIAAABqEOipAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABDSURBVGhD7c8BDQAgDMAw7F0nF4qOkSY10HNnP6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplem1zX7ANq7txGhH62zAAAAAElFTkSuQmCC"
        });

        dataService.Setup(x => x.SingleAsync<Setting>("document-header-signature")).ReturnsAsync(new Setting
        {
            Id = "document-header-signature",
            Value =
                "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEoAAAAlCAIAAABqEOipAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABDSURBVGhD7c8BDQAgDMAw7F0nF4qOkSY10HNnP6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplemV6ZXplem1zX7ANq7txGhH62zAAAAAElFTkSuQmCC"
        });

        dataService.Setup(x => x.SingleAsync<Setting>("document-header-your-name")).ReturnsAsync(new Setting
        {
            Id = "document-header-your-name",
            Value = "Simon Baudart"
        });

        dataService.Setup(x => x.SingleAsync<Setting>("document-header-your-city")).ReturnsAsync(new Setting
        {
            Id = "document-header-your-city",
            Value = "Orcq"
        });

        dataService.Setup(x => x.SingleAsync<Setting>("document-header-accentuate-color")).ReturnsAsync(new Setting
        {
            Id = "document-header-accentuate-color",
            Value = "#123456"
        });
    }
}