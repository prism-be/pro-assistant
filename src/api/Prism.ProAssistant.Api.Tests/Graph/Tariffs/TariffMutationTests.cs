// -----------------------------------------------------------------------
//  <copyright file = "TariffMutationTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Api.Graph.Tariffs;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;
using Prism.ProAssistant.UnitTesting;
using Prism.ProAssistant.UnitTesting.Fakes;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Graph.Tariffs;

public class TariffMutationTests
{
    [Fact]
    public async Task RemoveTariffsync_Ok()
    {
        // Arrange
        var patientId = Identifier.GenerateString();
        var database = new Mock<IMongoDatabase>();
        database.SetupCollection(new Tariff
            {
                Id = Identifier.GenerateString(),
                Name = Identifier.GenerateString(),
                Price = 42
            },
            new Tariff
            {
                Id = patientId,
                Name = Identifier.GenerateString(),
                Price = 42
            });

        var organisationContext = new Mock<IOrganizationContext>();
        organisationContext.Setup(x => x.Tariffs).Returns(database.Object.GetCollection<Tariff>());

        // Act
        var query = new TariffMutation();
        var result = await query.RemoveTariffAsync(patientId, organisationContext.Object, Mock.Of<ILogger<TariffMutation>>());

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpsertTariffsync_Existing()
    {
        // Arrange
        var replaceTarif = new Tariff
        {
            Name = Identifier.GenerateString(),
            Price = 42
        };

        var organisationContext = new OrganizationContextFake();

        // Act
        var query = new TariffMutation();
        var result = await query.UpsertTariffAsync(replaceTarif, organisationContext, Mock.Of<ILogger<TariffMutation>>(), Mock.Of<IUserContextAccessor>());

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task UpsertTariffsync_New()
    {
        // Arrange
        var replaceTarif = new Tariff
        {
            Price = 42
        };

        var organisationContext = new OrganizationContextFake();

        // Act
        var query = new TariffMutation();
        var result = await query.UpsertTariffAsync(replaceTarif, organisationContext, Mock.Of<ILogger<TariffMutation>>(), Mock.Of<IUserContextAccessor>());

        // Assert
        result.Should().NotBeNull();
    }
}