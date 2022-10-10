// -----------------------------------------------------------------------
//  <copyright file = "PatientMutations.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MongoDB.Driver;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Api.Mutations;

[ExtendObjectType(Name = "Mutation")]
public class PatientMutation
{
    public async Task<Patient> CreatePatientAsync(Patient patient, [Service] IOrganizationContext organizationContext)
    {
        await organizationContext.Patients.InsertOneAsync(patient);

        return patient;
    }

    public async Task<bool> RemovePatientAsync(Guid id, [Service] IOrganizationContext organizationContext)
    {
        var result = await organizationContext.Patients.DeleteOneAsync(Builders<Patient>.Filter.Eq("id", id));

        return result.IsAcknowledged;
    }
}