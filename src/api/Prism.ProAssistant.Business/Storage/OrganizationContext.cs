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
    IMongoCollection<Meeting> Meetings { get; }
    IMongoCollection<Patient> Patients { get; }
    IMongoCollection<Setting> Settings { get; }
    IMongoCollection<Tariff> Tariffs { get; }
}

public class OrganizationContext : IOrganizationContext
{
    public OrganizationContext(MongoDbConfiguration mongoDbConfiguration, IUserContextAccessor userContextAccessor)
    {
        var client = new MongoClient(mongoDbConfiguration.ConnectionString);

        var database = string.IsNullOrWhiteSpace(userContextAccessor.OrganisationId)
            ? client.GetDatabase("unknown")
            : client.GetDatabase(userContextAccessor.OrganisationId);

        History = database.GetCollection<History>(CollectionNames.History);
        Patients = database.GetCollection<Patient>(CollectionNames.Patients);
        Meetings = database.GetCollection<Meeting>(CollectionNames.Meetings);
        Settings = database.GetCollection<Setting>(CollectionNames.Settings);
        Tariffs = database.GetCollection<Tariff>(CollectionNames.Tariffs);
    }

    public IMongoCollection<History> History { get; }
    public IMongoCollection<Patient> Patients { get; }
    public IMongoCollection<Meeting> Meetings { get; }
    public IMongoCollection<Setting> Settings { get; }
    public IMongoCollection<Tariff> Tariffs { get; }

    public static class CollectionNames
    {
        public const string History = "history";
        public const string Meetings = "meetings";
        public const string Patients = "patients";
        public const string Settings = "settings";
        public const string Tariffs = "tariffs";
    }
}