// -----------------------------------------------------------------------
//  <copyright file = "PatientRepository.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MongoDB.Driver;
using Prism.ProAssistant.Business.Models;

namespace Prism.ProAssistant.Business.Storage;

public interface IPatientRepository
{
    Task<IEnumerable<Patient>> GetAllAsync();
    Task<Patient> GetByIdAsync(Guid id);
}

public class PatientRepository : IPatientRepository
{
    private readonly IOrganizationContext _organizationContext;

    public PatientRepository(IOrganizationContext organizationContext)
    {
        _organizationContext = organizationContext ?? throw new ArgumentNullException(nameof(organizationContext));
    }

    public async Task<IEnumerable<Patient>> GetAllAsync()
    {
        return await _organizationContext.Patients.Find(_ => true).ToListAsync();
    }

    public async Task<Patient> GetByIdAsync(Guid id)
    {
        var filter = Builders<Patient>.Filter.Eq(_ => _.Id, id);

        return await _organizationContext.Patients.Find(filter).FirstOrDefaultAsync();
    }
}