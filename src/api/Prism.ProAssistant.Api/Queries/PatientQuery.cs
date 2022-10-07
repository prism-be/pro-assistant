// -----------------------------------------------------------------------
//  <copyright file = "PatientQuery.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using HotChocolate.Data;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Api.Queries;

public class PatientQuery
{
    [UseFirstOrDefault]
    public IExecutable<Patient> GetPatientById(Guid id, [Service] IOrganizationContext organizationContext)
    {
        return organizationContext.Patients.Find(x => x.Id == id).AsExecutable();
    }

    [UsePaging]
    [UseProjection]
    [UseSorting]
    [UseFiltering]
    public IExecutable<Patient> GetPatients([Service] IOrganizationContext organizationContext)
    {
        return organizationContext.Patients.AsExecutable();
    }
}