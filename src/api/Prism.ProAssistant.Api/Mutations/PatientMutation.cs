// -----------------------------------------------------------------------
//  <copyright file = "PatientMutations.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MongoDB.Driver;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Api.Mutations;

[ExtendObjectType(Name = "Mutation")]
public class PatientMutation
{
    public async Task<Patient> CreatePatientAsync(Patient patient, [Service] IOrganizationContext organizationContext)
    {
        patient.Id = Identifier.Generate();
        await organizationContext.Patients.InsertOneAsync(patient);

        return patient;
    }
    
    public async Task<Patient> UpdatePatientAsync(Patient patient, [Service] IOrganizationContext organizationContext)
    {
        return await organizationContext.Patients.FindOneAndReplaceAsync(Builders<Patient>.Filter.Eq("Id", patient.Id), patient);
    }

    public async Task<bool> RemovePatientAsync(Guid id, [Service] IOrganizationContext organizationContext)
    {
        var result = await organizationContext.Patients.DeleteOneAsync(Builders<Patient>.Filter.Eq("Id", id));

        return result.IsAcknowledged;
    }
}