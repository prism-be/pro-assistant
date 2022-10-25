// -----------------------------------------------------------------------
//  <copyright file = "TarifMutationTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Api.Graph.Tariffs;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Graph.Tariffs;

public class TariffMutationTests
{
    [Fact]
    public async Task RemoveTariffsync_Ok()
    {
        // Arrange
        var patientId = Identifier.Generate();
        var database = new Mock<IMongoDatabase>();
        database.SetupCollection(new Tariff
            {
                Id = Identifier.Generate(),
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
        var result = await query.RemoveTariffAsync(patientId, organisationContext.Object);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpsertTariffsync_Ok()
    {
        // Arrange
        var patientId = Identifier.Generate();
        var replaceTarif = new Tariff
        {
            Id = patientId,
            Name = Identifier.GenerateString(),
            Price = 42
        };

        var database = new Mock<IMongoDatabase>();
        database.SetupCollectionAndReplace(replaceTarif, new Tariff
        {
            Id = patientId,
            Name = Identifier.GenerateString(),
            Price = 42
        });

        var organisationContext = new Mock<IOrganizationContext>();
        organisationContext.Setup(x => x.Tariffs).Returns(database.Object.GetCollection<Tariff>());

        // Act
        var query = new TariffMutation();
        var result = await query.UpsertTariffAsync(replaceTarif, organisationContext.Object);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(patientId);
    }
}