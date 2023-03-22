using FluentAssertions;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Api.Controllers.Data;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;

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

        var dataService = new Mock<IDataService>();

        // Act
        var controller = new AppointmentController(dataService.Object);
        await controller.Insert(appointment);

        // Assert
        dataService.Verify(x => x.InsertAsync(appointment), Times.Once);
        dataService.Verify(x => x.InsertAsync(It.Is<Contact>(c => c.FirstName == appointment.FirstName && c.LastName == appointment.LastName)));
        appointment.ContactId.Should().NotBeNull();
    }

    [Fact]
    public async Task Appointment_Ok()
    {
        await TestCrud(service => new AppointmentController(service.Object), () => new Appointment
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

        var dataService = new Mock<IDataService>();

        // Act
        var controller = new AppointmentController(dataService.Object);
        await controller.Update(appointment);

        // Assert
        dataService.Verify(x => x.ReplaceAsync(appointment), Times.Once);
        dataService.Verify(x => x.InsertAsync(It.Is<Contact>(c => c.FirstName == appointment.FirstName && c.LastName == appointment.LastName)));
        appointment.ContactId.Should().NotBeNull();
    }

    [Fact]
    public async Task Contact_Ok()
    {
        await TestCrud(service => new ContactController(service.Object), () => new Contact
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

        var dataService = await CheckUpdate(service => new ContactController(service.Object), () => new Contact
        {
            Id = id,
            FirstName = Identifier.GenerateString(),
            LastName = Identifier.GenerateString(),
            Title = Identifier.GenerateString()
        });

        dataService.Verify(x => x.UpdateManyAsync(It.Is<FilterDefinition<Appointment>>(t => Check(t, nameof(Appointment.ContactId), id)), It.IsAny<UpdateDefinition<Appointment>>()), Times.Once);
    }

    [Fact]
    public async Task DocumentConfiguration_Delete()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var mock = new Mock<IDataService>();
        var controller = new DocumentConfigurationController(mock.Object);

        // Act
        await controller.Delete(id);

        // Assert
        mock.Verify(x => x.DeleteAsync<DocumentConfiguration>(id), Times.Once);
    }

    [Fact]
    public async Task DocumentConfiguration_Ok()
    {
        await TestCrud(service => new DocumentConfigurationController(service.Object), () => new DocumentConfiguration
        {
            Id = Identifier.GenerateString(),
            Title = Identifier.GenerateString()
        });
    }

    [Fact]
    public async Task Setting_Ok()
    {
        await TestCrud(service => new SettingController(service.Object), () => new Setting
        {
            Id = Identifier.GenerateString()
        });
    }

    [Fact]
    public async Task Setting_UpdateMany()
    {
        // Arrange
        var mock = new Mock<IDataService>();
        var controller = new SettingController(mock.Object);

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
        mock.Verify(x => x.ReplaceManyAsync(It.IsAny<List<Setting>>()), Times.Once);
    }

    [Fact]
    public async Task Tariff_Ok()
    {
        await TestCrud(service => new TariffController(service.Object), () => new Tariff
        {
            Id = Identifier.GenerateString(),
            Name = Identifier.GenerateString()
        });
    }

    [Fact]
    public async Task Tariff_Update_Appointments()
    {
        var id = Identifier.GenerateString();

        var dataService = await CheckUpdate(service => new TariffController(service.Object), () => new Tariff
        {
            Id = id,
            Name = Identifier.GenerateString()
        });

        dataService.Verify(x => x.UpdateManyAsync(It.Is<FilterDefinition<Appointment>>(t => Check(t, nameof(Appointment.TypeId), id)), It.IsAny<UpdateDefinition<Appointment>>()), Times.Once);
    }

    private static bool Check<T>(FilterDefinition<T> filterDefinition, string propertyName, string value) where T : IDataModel
    {
        var serializerRegistry = BsonSerializer.SerializerRegistry;
        var documentSerializer = serializerRegistry.GetSerializer<T>();

        var rendered = filterDefinition.Render(documentSerializer, serializerRegistry);

        return rendered[propertyName]?.ToString() == value;
    }

    private async static Task CheckInsert<T>(Func<Mock<IDataService>, IDataController<T>> factory, Func<T> itemFactory) where T : IDataModel
    {
        var mock = new Mock<IDataService>();
        var controller = factory(mock);

        var item = itemFactory();
        await controller.Insert(item);
        mock.Verify(x => x.InsertAsync(item), Times.Once);
    }

    private async static Task CheckList<T>(Func<Mock<IDataService>, IDataController<T>> factory, Func<T> itemFactory) where T : IDataModel
    {
        var mock = new Mock<IDataService>();
        mock.Setup(x => x.ListAsync<T>())
            .ReturnsAsync(new List<T>
            {
                itemFactory(),
                itemFactory()
            });
        var controller = factory(mock);
        var result = await controller.List();

        result.Count.Should().Be(2);
        mock.Verify(x => x.ListAsync<T>(), Times.Once);
    }

    private async static Task CheckSearch<T>(Func<Mock<IDataService>, IDataController<T>> factory, Func<T> itemFactory) where T : IDataModel
    {
        var filters = new List<SearchFilter>();
        var mock = new Mock<IDataService>();
        mock.Setup(x => x.SearchAsync<T>(filters))
            .ReturnsAsync(new List<T>
            {
                itemFactory(),
                itemFactory()
            });
        var controller = factory(mock);

        var results = await controller.Search(filters);
        mock.Verify(x => x.SearchAsync<T>(filters), Times.Once);
        results.Count.Should().Be(2);
    }

    private async static Task CheckSingle<T>(Func<Mock<IDataService>, IDataController<T>> factory, Func<T> itemFactory) where T : IDataModel
    {
        var id = Identifier.GenerateString();
        var mock = new Mock<IDataService>();
        mock.Setup(x => x.SingleOrDefaultAsync<T>(id))!.ReturnsAsync(itemFactory);
        var controller = factory(mock);
        var result = await controller.Single(id);
        result.Should().NotBeNull();
        mock.Verify(x => x.SingleOrDefaultAsync<T>(id), Times.Once);
    }

    private async static Task<Mock<IDataService>> CheckUpdate<T>(Func<Mock<IDataService>, IDataController<T>> factory, Func<T> itemFactory) where T : IDataModel
    {
        var mock = new Mock<IDataService>();
        var controller = factory(mock);

        var item = itemFactory();
        await controller.Update(item);
        mock.Verify(x => x.ReplaceAsync(item), Times.Once);

        return mock;
    }

    private async Task TestCrud<T>(Func<Mock<IDataService>, IDataController<T>> factory, Func<T> itemFactory) where T : IDataModel
    {
        await CheckUpdate(factory, itemFactory);
        await CheckInsert(factory, itemFactory);
        await CheckList(factory, itemFactory);
        await CheckSearch(factory, itemFactory);
        await CheckSingle(factory, itemFactory);
    }
}