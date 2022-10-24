// -----------------------------------------------------------------------
//  <copyright file = "PatientType.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.ProAssistant.Business.Models;

namespace Prism.ProAssistant.Api.Graph.Tarifs;

public class TarifType : ObjectType<Tarif>
{
    public TarifType() : base(ConfigureTarif)
    {
    }

    internal static void ConfigureTarif(IObjectTypeDescriptor<Tarif> descriptor)
    {
        descriptor.Field(_ => _.Id);
        descriptor.Field(_ => _.Name);
        descriptor.Field(_ => _.Price);
    }
}