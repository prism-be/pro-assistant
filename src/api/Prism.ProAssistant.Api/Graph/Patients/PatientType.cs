// -----------------------------------------------------------------------
//  <copyright file = "PatientType.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using HotChocolate.AspNetCore.Authorization;
using Prism.ProAssistant.Business.Models;

namespace Prism.ProAssistant.Api.Graph.Patients;

public class PatientType: ObjectType<Patient>
{
    protected override void Configure(IObjectTypeDescriptor<Patient> descriptor)
    {
        descriptor.Field(_ => _.Id);
        descriptor.Field(_ => _.FirstName);
        descriptor.Field(_ => _.LastName);
        descriptor.Field(_ => _.Street);
        descriptor.Field(_ => _.Number);
        descriptor.Field(_ => _.ZipCode);
        descriptor.Field(_ => _.City);
        descriptor.Field(_ => _.Country);
        descriptor.Field(_ => _.BirthDate);
        
        base.Configure(descriptor);
    }
}