// -----------------------------------------------------------------------
//  <copyright file = "PatientMutations.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using HotChocolate.AspNetCore.Authorization;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Api.Graph.Patients;

[Authorize]
[ExtendObjectType("Mutation")]
public class PatientMutation
{
    public async Task<Patient> CreatePatientAsync(Patient patient, [Service] IOrganizationContext organizationContext, [Service] ILogger<PatientMutation> logger)
    {
        patient.Id = Identifier.Generate();
        
        logger.LogInformation("Creating an new patient {patientId}", patient.Id);
        
        await organizationContext.Patients.InsertOneAsync(patient);

        return patient;
    }
    
    public async Task<Patient> UpdatePatientAsync(Patient patient, [Service] IOrganizationContext organizationContext, [Service] ILogger<PatientMutation> logger)
    {
        logger.LogInformation("Updating an existing patient {patientId}", patient.Id);
        return await organizationContext.Patients.FindOneAndReplaceAsync(Builders<Patient>.Filter.Eq("Id", patient.Id), patient);
    }
}