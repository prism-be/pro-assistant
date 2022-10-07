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
    IMongoCollection<Patient> Patients { get; }
}

public class OrganizationContext : IOrganizationContext
{
    private const string PatientsCollectionName = "patients";

    public OrganizationContext(MongoDbConfiguration mongoDbConfiguration, IUserContextAccessor userContextAccessor)
    {
        var client = new MongoClient(mongoDbConfiguration.ConnectionString);
        var database = client.GetDatabase(userContextAccessor.OrganisationId.ToString());

        Patients = database.GetCollection<Patient>(PatientsCollectionName);
    }

    public IMongoCollection<Patient> Patients { get; }
}