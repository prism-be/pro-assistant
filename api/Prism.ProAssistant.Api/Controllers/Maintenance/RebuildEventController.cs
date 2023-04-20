using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Prism.Core.Attributes;
using Prism.Infrastructure.Authentication;
using Prism.ProAssistant.Domain;
using Prism.ProAssistant.Domain.Configuration.DocumentConfiguration;
using Prism.ProAssistant.Domain.Configuration.DocumentConfiguration.Events;
using Prism.ProAssistant.Domain.Configuration.Settings;
using Prism.ProAssistant.Domain.Configuration.Settings.Events;
using Prism.ProAssistant.Domain.Configuration.Tariffs;
using Prism.ProAssistant.Domain.Configuration.Tariffs.Events;
using Prism.ProAssistant.Domain.DayToDay.Appointments;
using Prism.ProAssistant.Domain.DayToDay.Appointments.Events;
using Prism.ProAssistant.Domain.DayToDay.Contacts;
using Prism.ProAssistant.Domain.DayToDay.Contacts.Events;
using Prism.ProAssistant.Storage;
using Prism.ProAssistant.Storage.Events;

namespace Prism.ProAssistant.Api.Controllers.Maintenance;

[Authorize]
public class RebuildEventController : Controller
{
    private readonly IEventStore _eventStore;
    private readonly ILogger<RebuildEventController> _logger;
    private readonly IQueryService _queryService;
    private readonly IMongoClient _mongoClient;
    private readonly UserOrganization _userOrganization;

    public RebuildEventController(IQueryService queryService, IEventStore eventStore, ILogger<RebuildEventController> logger, IMongoClient mongoClient, UserOrganization userOrganization)
    {
        _queryService = queryService;
        _eventStore = eventStore;
        _logger = logger;
        _mongoClient = mongoClient;
        _userOrganization = userOrganization;
    }

    [HttpPost]
    [Route("api/maintenance/rebuild-events")]
    public async Task Rebuild()
    {
        _logger.LogWarning("Rebuilding events for all collections");
        await RebuildEvents<AppointmentObjectId, Appointment>(item => new AppointmentCreated { Appointment = item });
        await RebuildEvents<ContactObjectId, Contact>(item => new ContactCreated
            { Contact = item });
        await RebuildEvents<DocumentConfigurationObjectId, DocumentConfiguration>(item => new DocumentConfigurationCreated
            { DocumentConfiguration = item });
        await RebuildEvents<SettingObjectId, Setting>(item => new SettingCreated
            { Setting = item });
        await RebuildEvents<TariffObjectId, Tariff>(item => new TariffCreated
            { Tariff = item });
    }

    [Collection("appointments.old")]
    class AppointmentObjectId
    {
        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("paymentDate")]
        public DateTime? PaymentDate { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("payment")]
        public int Payment { get; set; }

        [JsonPropertyName("state")]
        public int State { get; set; }

        [JsonPropertyName("documents")]
        public List<BinaryDocumentObjectId> Documents { get; set; } = new();

        [JsonPropertyName("firstName")]
        required public string FirstName { get; set; }

        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("lastName")]
        required public string LastName { get; set; }

        [JsonPropertyName("title")]
        required public string Title { get; set; }

        [JsonPropertyName("backgroundColor")]
        public string? BackgroundColor { get; set; }

        [JsonPropertyName("birthDate")]
        public string? BirthDate { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        [JsonPropertyName("contactId")]
        public string? ContactId { get; set; }

        [JsonPropertyName("foreColor")]
        public string? ForeColor { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("typeId")]
        public string? TypeId { get; set; }
    }
    
    [Collection("contacts.old")]
    class ContactObjectId
    {
        [JsonPropertyName("birthDate")]
        public string? BirthDate { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("mobileNumber")]
        public string? MobileNumber { get; set; }

        [JsonPropertyName("number")]
        public string? Number { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonPropertyName("street")]
        public string? Street { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("zipCode")]
        public string? ZipCode { get; set; }

        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        [JsonPropertyName("id")]
        required public string Id { get; set; }
    }
    
    [Collection("documents-configuration.old")]
    class DocumentConfigurationObjectId
    {

        [JsonPropertyName("body")]
        public string? Body { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        [JsonPropertyName("id")]
        required public string Id { get; set; }
    }
    
    public class BinaryDocumentObjectId
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("fileName")]
        required public string FileName { get; set; }

        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        [JsonPropertyName("id")]
        required public string Id { get; set; }

        [JsonPropertyName("title")]
        required public string Title { get; set; }
    }
    
    [Collection("settings.old")]
    class SettingObjectId
    {

        [JsonPropertyName("id")]
        required public string Id { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;
    }
    
    [Collection("tariffs.old")]
    public class TariffObjectId
    {

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("defaultDuration")]
        public int DefaultDuration { get; set; } = 60;

        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        [JsonPropertyName("id")]
        required public string Id { get; set; }

        [JsonPropertyName("name")]
        required public string Name { get; set; }

        [JsonPropertyName("backgroundColor")]
        public string? BackgroundColor { get; set; }

        [JsonPropertyName("foreColor")]
        public string? ForeColor { get; set; }
    }
    
    private async Task RebuildEvents<TFrom, TTo>(Func<TTo, IDomainEvent> factory)
    {
        _logger.LogInformation("Rebuilding events for {collection}", typeof(TTo).Name);
        
        var collectionName = CollectionAttribute.GetCollectionName<TFrom>();
        var collection = _mongoClient.GetDatabase(_userOrganization.Organization).GetCollection<TFrom>(collectionName);
        var items = collection.Find(FilterDefinition<TFrom>.Empty).ToEnumerable();

        foreach (var item in items)
        {
            var json = JsonSerializer.Serialize(item);
            var deserialized = JsonSerializer.Deserialize<TTo>(json);
            
            if (deserialized == null)
            {
                _logger.LogWarning("Unable to deserialize {item} to {type}", json, typeof(TTo).Name);
                continue;
            }
            
            await _eventStore.RaiseAndPersist<TTo>(factory(deserialized));
        }
    }
}