// -----------------------------------------------------------------------
//  <copyright file = "PatientType.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.ProAssistant.Business.Models;

namespace Prism.ProAssistant.Api.Graph.Tariffs;

public class TariffType : ObjectType<Tariff>
{
    public TariffType() : base(ConfigureTariff)
    {
    }

    internal static void ConfigureTariff(IObjectTypeDescriptor<Tariff> descriptor)
    {
        descriptor.Field(_ => _.Id);
        descriptor.Field(_ => _.Name);
        descriptor.Field(_ => _.Price);
    }
}