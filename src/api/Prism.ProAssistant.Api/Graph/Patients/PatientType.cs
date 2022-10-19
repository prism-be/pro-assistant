// -----------------------------------------------------------------------
//  <copyright file = "PatientType.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.ProAssistant.Business.Models;

namespace Prism.ProAssistant.Api.Graph.Patients;

public class PatientType: ObjectType<Patient>
{
    public PatientType() : base(ConfigurePatient)
    {
    }

    internal static void ConfigurePatient(IObjectTypeDescriptor<Patient> descriptor)
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
    }
}