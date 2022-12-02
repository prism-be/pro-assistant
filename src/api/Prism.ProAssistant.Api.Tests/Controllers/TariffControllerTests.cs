// -----------------------------------------------------------------------
//  <copyright file = "TariffControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using Prism.ProAssistant.Api.Controllers;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Controllers
{
    public class TariffControllerTests
    {
        [Fact]
        public async Task FindMany()
        {
            await CrudPublisherTests.FindMany<TariffController, Tariff>(c => c.FindMany());
        }

        [Fact]
        public async Task FindOne()
        {
            await CrudPublisherTests.FindOne<TariffController, Tariff>(c => c.FindOne(Identifier.GenerateString()));
        }

        [Fact]
        public async Task RemoveOne()
        {
            await CrudPublisherTests.RemoveOne<TariffController, Tariff>(c => c.RemoveOne(Identifier.GenerateString()));
        }

        [Fact]
        public async Task UpsertOne()
        {
            await CrudPublisherTests.UpsertOne<TariffController, Tariff>(c => c.UpsertOne(new Tariff
            {
                Id = Identifier.GenerateString()
            }));
        }
    }
}