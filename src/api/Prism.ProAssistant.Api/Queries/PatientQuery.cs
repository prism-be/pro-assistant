// -----------------------------------------------------------------------
//  <copyright file = "PatientQuery.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Api.Queries;

public class PatientQuery
{

    public Task<Patient> GetPatientById(Guid id, [Service] IPatientRepository patientRepository)
    {
        return patientRepository.GetByIdAsync(id);
    }

    public Task<IEnumerable<Patient>> GetPatientsAsync([Service] IPatientRepository patientRepository)
    {
        return patientRepository.GetAllAsync();
    }
}