using FluentAssertions;
using MediatR;
using Moq;
using Prism.ProAssistant.Api.Controllers.Data;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;
using Prism.ProAssistant.Storage.Events;

namespace Prism.ProAssistant.Api.Tests.Controllers;

public class DataControllersTests
{

    [Fact]
    public async Task Appointment_Insert_No_Contact()
    {
        // Arrange
        var appointment = new Appointment
        {
            Id = Identifier.GenerateString(),
            FirstName = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            Title = Identifier.GenerateString()
        };

        var dataService = new Mock<IQueryService>();
        var eventService = new Mock<IEventService>();
        eventService.Setup(x => x.CreateAsync(It.IsAny<Contact>()))
            .ReturnsAsync(new UpsertResult(Identifier.GenerateString()));

        // Act
        var controller = new AppointmentController(dataService.Object, eventService.Object);
        await controller.Insert(appointment);

        // Assert
        eventService.Verify(x => x.CreateAsync(appointment), Times.Once);
        eventService.Verify(x => x.CreateAsync(It.Is<Contact>(c => c.FirstName == appointment.FirstName && c.LastName == appointment.LastName)));
        appointment.ContactId.Should().NotBeNull();
    }

    [Fact]
    public async Task Appointment_Ok()
    {
        await TestCrud((dataService, eventService) => new AppointmentController(dataService.Object, eventService.Object), () => new Appointment
        {
            Id = Identifier.GenerateString(),
            FirstName = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            Title = Identifier.GenerateString(),
            ContactId = Identifier.GenerateString()
        });
    }

    [Fact]
    public async Task Appointment_Update_No_Contact()
    {
        // Arrange
        var appointment = new Appointment
        {
            Id = Identifier.GenerateString(),
            FirstName = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            Title = Identifier.GenerateString()
        };

        var dataService = new Mock<IQueryService>();
        var eventService = new Mock<IEventService>();
        eventService.Setup(x => x.CreateAsync(It.IsAny<Contact>()))
            .ReturnsAsync(new UpsertResult(Identifier.GenerateString()));

        // Act
        var controller = new AppointmentController(dataService.Object, eventService.Object);
        await controller.Update(appointment);

        // Assert
        eventService.Verify(x => x.ReplaceAsync(appointment), Times.Once);
        eventService.Verify(x => x.CreateAsync(It.Is<Contact>(c => c.FirstName == appointment.FirstName && c.LastName == appointment.LastName)));
        appointment.ContactId.Should().NotBeNull();
    }

    [Fact]
    public async Task Contact_Ok()
    {
        await TestCrud((dataService, eventService) => new ContactController(dataService.Object, eventService.Object, Mock.Of<IEventStore>()), () => new Contact
        {
            Id = Identifier.GenerateString(),
            FirstName = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            Title = Identifier.GenerateString()
        });
    }

    [Fact]
    public async Task Contact_Update_Appointments()
    {
        var id = Identifier.GenerateString();

        var eventService = await CheckUpdate((dataService, eventService) => new ContactController(dataService.Object, eventService.Object, Mock.Of<IEventStore>()), () => new Contact
        {
            Id = id,
            FirstName = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            Title = Identifier.GenerateString()
        });

        eventService.Verify(x => x.UpdateManyAsync<Appointment>(It.Is<FieldValue>(t => t.Field == nameof(Appointment.ContactId) && t.Value!.ToString() == id), It.IsAny<FieldValue[]>()), Times.Once);
    }

    [Fact]
    public async Task DocumentConfiguration_Delete()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var mockDataService = new Mock<IQueryService>();
        var mockEventService = new Mock<IEventService>();
        var controller = new DocumentConfigurationController(mockDataService.Object, mockEventService.Object);

        // Act
        await controller.Delete(id);

        // Assert
        mockEventService.Verify(x => x.DeleteAsync<DocumentConfiguration>(id), Times.Once);
    }

    [Fact]
    public async Task DocumentConfiguration_Ok()
    {
        await TestCrud((dataService, eventService) => new DocumentConfigurationController(dataService.Object, eventService.Object), () => new DocumentConfiguration
        {
            Id = Identifier.GenerateString(),
            Title = Identifier.GenerateString()
        });
    }

    [Fact]
    public async Task Setting_Ok()
    {
        await TestCrud((dataService, eventService) => new SettingController(dataService.Object, eventService.Object), () => new Setting
        {
            Id = Identifier.GenerateString()
        });
    }

    [Fact]
    public async Task Setting_UpdateMany()
    {
        // Arrange
        var mockDataService = new Mock<IQueryService>();
        var mockEventService = new Mock<IEventService>();
        var controller = new SettingController(mockDataService.Object, mockEventService.Object);

        // Act
        await controller.UpdateMany(new List<Setting>
        {
            new()
            {
                Id = Identifier.GenerateString()
            },
            new()
            {
                Id = Identifier.GenerateString()
            }
        });

        // Assert
        mockEventService.Verify(x => x.ReplaceManyAsync(It.IsAny<List<Setting>>()), Times.Once);
    }

    [Fact]
    public async Task Tariff_Ok()
    {
        await TestCrud((dataService, eventService) => new TariffController(dataService.Object, eventService.Object), () => new Tariff
        {
            Id = Identifier.GenerateString(),
            Name = Identifier.GenerateString()
        });
    }

    [Fact]
    public async Task Tariff_Update_Appointments()
    {
        var id = Identifier.GenerateString();

        var eventService = await CheckUpdate((dataService, eventService) => new TariffController(dataService.Object, eventService.Object), () => new Tariff
        {
            Id = id,
            Name = Identifier.GenerateString()
        });

        eventService.Verify(x => x.UpdateManyAsync<Appointment>(It.Is<FieldValue>(t => t.Field == nameof(Appointment.TypeId) && t.Value!.ToString() == id), It.IsAny<FieldValue[]>()), Times.Once);
    }

    private async static Task CheckInsert<T>(Func<Mock<IQueryService>, Mock<IEventService>, IDataController<T>> factory, Func<T> itemFactory) where T : IDataModel
    {
        var mockDataService = new Mock<IQueryService>();
        var mockEventService = new Mock<IEventService>();
        var controller = factory(mockDataService, mockEventService);

        var item = itemFactory();
        await controller.Insert(item);
        mockEventService.Verify(x => x.CreateAsync(item), Times.Once);
    }

    private async static Task CheckList<T>(Func<Mock<IQueryService>, Mock<IEventService>, IDataController<T>> factory, Func<T> itemFactory) where T : IDataModel
    {
        var mockDataService = new Mock<IQueryService>();
        mockDataService.Setup(x => x.ListAsync<T>())
            .ReturnsAsync(new List<T>
            {
                itemFactory(),
                itemFactory()
            });
        var mockEventService = new Mock<IEventService>();
        var controller = factory(mockDataService, mockEventService);
        var result = await controller.List();

        result.Count.Should().Be(2);
        mockDataService.Verify(x => x.ListAsync<T>(), Times.Once);
    }

    private async static Task CheckSearch<T>(Func<Mock<IQueryService>, Mock<IEventService>, IDataController<T>> factory, Func<T> itemFactory) where T : IDataModel
    {
        var filters = new List<SearchFilter>();
        var mockDataService = new Mock<IQueryService>();
        mockDataService.Setup(x => x.SearchAsync<T>(filters))
            .ReturnsAsync(new List<T>
            {
                itemFactory(),
                itemFactory()
            });
        var mockEventService = new Mock<IEventService>();
        var controller = factory(mockDataService, mockEventService);

        var results = await controller.Search(filters);
        mockDataService.Verify(x => x.SearchAsync<T>(filters), Times.Once);
        results.Count.Should().Be(2);
    }

    private async static Task CheckSingle<T>(Func<Mock<IQueryService>, Mock<IEventService>, IDataController<T>> factory, Func<T> itemFactory) where T : IDataModel
    {
        var id = Identifier.GenerateString();
        var mockDataService = new Mock<IQueryService>();
        mockDataService.Setup(x => x.SingleOrDefaultAsync<T>(id))!.ReturnsAsync(itemFactory);
        var mockEventService = new Mock<IEventService>();
        var controller = factory(mockDataService, mockEventService);
        var result = await controller.Single(id);
        result.Should().NotBeNull();
        mockDataService.Verify(x => x.SingleOrDefaultAsync<T>(id), Times.Once);
    }

    private async static Task<Mock<IEventService>> CheckUpdate<T>(Func<Mock<IQueryService>, Mock<IEventService>, IDataController<T>> factory, Func<T> itemFactory) where T : IDataModel
    {
        var mockDataService = new Mock<IQueryService>();
        var mockEventService = new Mock<IEventService>();
        var controller = factory(mockDataService, mockEventService);

        var item = itemFactory();
        await controller.Update(item);
        mockEventService.Verify(x => x.ReplaceAsync(item), Times.Once);

        return mockEventService;
    }

    private async static Task TestCrud<T>(Func<Mock<IQueryService>, Mock<IEventService>, IDataController<T>> factory, Func<T> itemFactory) where T : IDataModel
    {
        await CheckUpdate(factory, itemFactory);
        await CheckInsert(factory, itemFactory);
        await CheckList(factory, itemFactory);
        await CheckSearch(factory, itemFactory);
        await CheckSingle(factory, itemFactory);
    }
}