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
    public async Task<Patient> CreatePatientAsync(Patient patient, [Service] IOrganizationContext organizationContext, [Service] ILogger<PatientMutation> logger, [Service] IUserContextAccessor userContextAccessor)
    {
        logger.LogInformation("Creating an new patient {patientId}", patient.Id);
        
        await organizationContext.Patients.InsertOneAsync(patient);
        await organizationContext.History.InsertOneAsync(new History(userContextAccessor.UserId, patient));

        return patient;
    }
    
    public async Task<Patient> UpdatePatientAsync(Patient patient, [Service] IOrganizationContext organizationContext, [Service] ILogger<PatientMutation> logger, [Service] IUserContextAccessor userContextAccessor)
    {
        logger.LogInformation("Updating an existing patient {patientId}", patient.Id);
        await organizationContext.History.InsertOneAsync(new History(userContextAccessor.UserId, patient));
        return await organizationContext.Patients.FindOneAndReplaceAsync(Builders<Patient>.Filter.Eq("Id", patient.Id), patient);
    }
}