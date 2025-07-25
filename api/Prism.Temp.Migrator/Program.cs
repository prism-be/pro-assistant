using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Prism.Core;
using Prism.ProAssistant.Domain.DayToDay.Contacts;
using ProAssistantDbContext = Prism.Temp.Migrator.ProAssistantDbContext;

var mongoDbConnectionString = EnvironmentConfiguration.GetMandatoryConfiguration("MONGODB_CONNECTION_STRING");
var mongoClient = new MongoClient(mongoDbConnectionString);

var dbContext = new ProAssistantDbContext();

var mongo = mongoClient.GetDatabase("cieletsens");

var contactsCollection = mongo.GetCollection<Contact>("contacts");
var contacts = await contactsCollection.Find(_ => true).ToListAsync();

foreach (var contact in contacts)
{
    var existingContact = await dbContext.Contacts
        .FirstOrDefaultAsync(c => c.Id == contact.Id);

    if (existingContact is not null)
    {
        Console.WriteLine($"Updating {contact.FirstName} {contact.LastName}");
        dbContext.Contacts.Update(existingContact);
    }
    else
    {
        Console.WriteLine($"Adding {contact.FirstName} {contact.LastName}");
        dbContext.Contacts.Add(contact);
    }
    
    dbContext.SaveChanges();
}