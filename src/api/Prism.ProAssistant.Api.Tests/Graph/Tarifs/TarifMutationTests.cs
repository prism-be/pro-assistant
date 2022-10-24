// -----------------------------------------------------------------------
//  <copyright file = "TarifMutationTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Api.Graph.Tarifs;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Graph.Tarifs;

public class TarifMutationTests
{
    [Fact]
    public async Task CreateTarifAsync_Ok()
    {
        // Arrange
        var tarifId = Identifier.Generate();
        var database = new Mock<IMongoDatabase>();
        database.SetupCollection(new Tarif
        {
            Id = Identifier.Generate(),
            Name = Identifier.GenerateString(),
            Price = 42
        });

        var organisationContext = new Mock<IOrganizationContext>();
        organisationContext.Setup(x => x.Tarifs).Returns(database.Object.GetCollection<Tarif>());

        // Act
        var query = new TarifMutation();
        var result = await query.CreateTarifAsync(new Tarif
        {
            Id = tarifId,
            Name = Identifier.GenerateString(),
            Price = 42.01M
        }, organisationContext.Object);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBe(tarifId);
    }

    [Fact]
    public async Task RemoveTarifAsync_Ok()
    {
        // Arrange
        var patientId = Identifier.Generate();
        var database = new Mock<IMongoDatabase>();
        database.SetupCollection(new Tarif
            {
                Id = Identifier.Generate(),
                Name = Identifier.GenerateString(),
                Price = 42
            },
            new Tarif
            {
                Id = patientId,
                Name = Identifier.GenerateString(),
                Price = 42
            });

        var organisationContext = new Mock<IOrganizationContext>();
        organisationContext.Setup(x => x.Tarifs).Returns(database.Object.GetCollection<Tarif>());

        // Act
        var query = new TarifMutation();
        var result = await query.RemoveTarifAsync(patientId, organisationContext.Object);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateTarifAsync_Ok()
    {
        // Arrange
        var patientId = Identifier.Generate();
        var replaceTarif = new Tarif
        {
            Id = patientId,
            Name = Identifier.GenerateString(),
            Price = 42
        };

        var database = new Mock<IMongoDatabase>();
        database.SetupCollectionAndReplace(replaceTarif, new Tarif
        {
            Id = patientId,
            Name = Identifier.GenerateString(),
            Price = 42
        });

        var organisationContext = new Mock<IOrganizationContext>();
        organisationContext.Setup(x => x.Tarifs).Returns(database.Object.GetCollection<Tarif>());

        // Act
        var query = new TarifMutation();
        var result = await query.UpdateTarifAsync(replaceTarif, organisationContext.Object);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(patientId);
    }
}