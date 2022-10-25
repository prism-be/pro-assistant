// -----------------------------------------------------------------------
//  <copyright file = "OrganizationContext.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MongoDB.Driver;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;

namespace Prism.ProAssistant.Business.Storage;

public interface IOrganizationContext
{
    IMongoCollection<History> History { get; }
    IMongoCollection<Patient> Patients { get; }
    IMongoCollection<Tariff> Tariffs { get; }
}

public class OrganizationContext : IOrganizationContext
{

    private static readonly Collation CaseInsensitiveCollation = new("fr", strength: CollationStrength.Primary);

    public OrganizationContext(MongoDbConfiguration mongoDbConfiguration, IUserContextAccessor userContextAccessor)
    {
        var client = new MongoClient(mongoDbConfiguration.ConnectionString);
        var database = client.GetDatabase(userContextAccessor.OrganisationId);

        if (!database.ListCollectionNames().Any())
        {
            Initialize(database);
        }

        History = database.GetCollection<History>(CollectionNames.History);
        Patients = database.GetCollection<Patient>(CollectionNames.Patients);
        Tariffs = database.GetCollection<Tariff>(CollectionNames.Tariffs);
    }

    public IMongoCollection<Tariff> Tariffs { get; }

    public IMongoCollection<Patient> Patients { get; }

    public IMongoCollection<History> History { get; }

    private void Initialize(IMongoDatabase database)
    {
        database.CreateCollection(CollectionNames.Patients, new CreateCollectionOptions
        {
            Collation = CaseInsensitiveCollation
        });

        var firstNameIndexModel = new CreateIndexModel<Patient>(Builders<Patient>.IndexKeys.Ascending(x => x.FirstName));
        database.GetCollection<Patient>(CollectionNames.Patients).Indexes.CreateOne(firstNameIndexModel);
        var lastNameIndexModel = new CreateIndexModel<Patient>(Builders<Patient>.IndexKeys.Ascending(x => x.LastName));
        database.GetCollection<Patient>(CollectionNames.Patients).Indexes.CreateOne(lastNameIndexModel);
    }

    public static class CollectionNames
    {
        public const string Patients = "patients";
        public const string Tariffs = "tariffs";
        public const string History = "history";
    }
}