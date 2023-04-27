using Moq;
using Prism.Core;
using Prism.Infrastructure.Providers;
using Prism.ProAssistant.Api.Controllers.Data;
using Prism.ProAssistant.Domain.DayToDay.Contacts;
using Prism.ProAssistant.Domain.DayToDay.Contacts.Events;
using Prism.ProAssistant.Storage;
using Prism.ProAssistant.Storage.Events;

namespace Prism.ProAssistant.Api.Tests.Controllers.Data;

using Domain;

public class ContactControllerTests
{
    [Fact]
    public async Task Insert_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();
        var contact = new Contact
        {
            Id = Identifier.GenerateString()
        };

        // Act
        var controller = new ContactController(queryService.Object, eventStore.Object);
        await controller.Insert(contact);

        // Assert
        eventStore.Verify(x => x.RaiseAndPersist<Contact>(It.Is<ContactCreated>(y => y.Contact == contact)), Times.Once);
    }

    [Fact]
    public async Task List_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        // Act
        var controller = new ContactController(queryService.Object, eventStore.Object);
        await controller.List();

        // Assert
        queryService.Verify(x => x.ListAsync<Contact>(), Times.Once);
    }

    [Fact]
    public async Task Search_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        // Act
        var controller = new ContactController(queryService.Object, eventStore.Object);
        await controller.Search(Array.Empty<Filter>());

        // Assert
        queryService.Verify(x => x.SearchAsync<Contact>(It.IsAny<Filter[]>()), Times.Once);
    }

    [Fact]
    public async Task Single_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        var id = Identifier.GenerateString();

        // Act
        var controller = new ContactController(queryService.Object, eventStore.Object);
        await controller.Single(id);

        // Assert
        queryService.Verify(x => x.SingleAsync<Contact>(id), Times.Once);
    }

    [Fact]
    public async Task Update_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        var eventStore = new Mock<IEventStore>();

        var contact = new Contact
        {
            Id = Identifier.GenerateString()
        };
        
        var previous = new Contact
        {
            Id = contact.Id
        };
        
        queryService.Setup(x => x.SingleAsync<Contact>(contact.Id)).ReturnsAsync(previous);

        // Act
        var controller = new ContactController(queryService.Object, eventStore.Object);
        await controller.Update(contact);

        // Assert
        eventStore.Verify(x => x.RaiseAndPersist<Contact>(It.Is<ContactUpdated>(y => y.Contact == contact)), Times.Once);
    }
}