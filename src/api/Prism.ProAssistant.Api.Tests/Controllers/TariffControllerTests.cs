// -----------------------------------------------------------------------
//  <copyright file = "TariffControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.ProAssistant.Api.Controllers;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Controllers;

public class TariffControllerTests
{
    [Fact]
    public async Task FindMany()
    {
        await CrudTests.FindMany<TariffController, Tariff>(c => c.FindMany());
    }

    [Fact]
    public async Task FindOne()
    {
        await CrudTests.FindOne<TariffController, Tariff>(c => c.FindOne(Identifier.GenerateString()));
    }

    [Fact]
    public async Task RemoveOne()
    {
        await CrudTests.RemoveOne<TariffController, Tariff>(c => c.RemoveOne(Identifier.GenerateString()));
    }

    [Fact]
    public async Task UpsertOne()
    {
        await CrudTests.UpsertOne<TariffController, Tariff>(c => c.UpsertOne(new Tariff
        {
            Id = Identifier.GenerateString(),
            Name = Identifier.GenerateString()
        }));
    }
}