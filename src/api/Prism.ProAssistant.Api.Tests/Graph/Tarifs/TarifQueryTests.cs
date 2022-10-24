// -----------------------------------------------------------------------
//  <copyright file = "TarifQueryTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading;
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

public class TarifQueryTests
{
    [Fact]
    public async Task GetTarifById_Ok()
    {
        // Arrange
        var tarifId = Identifier.Generate();
        var database = new Mock<IMongoDatabase>();
        database.SetupCollection(new Tarif
        {
            Id = tarifId,
            Name = Identifier.GenerateString(),
            Price = 42
        });

        var organisationContext = new Mock<IOrganizationContext>();
        organisationContext.Setup(x => x.Tarifs).Returns(database.Object.GetCollection<Tarif>());

        // Act
        var query = new TarifQuery();
        var result = query.GetTarifById(tarifId, organisationContext.Object);
        var patient = await result.SingleOrDefaultAsync(CancellationToken.None) as Tarif;

        // Assert
        patient.Should().NotBeNull();
        patient?.Id.Should().Be(tarifId);
    }

    [Fact]
    public void GetTarifs_Ok()
    {
        // Arrange
        var database = new Mock<IMongoDatabase>();
        database.SetupCollection(new Tarif
            {
                Id = Identifier.Generate(),
                Name = Identifier.GenerateString(),
                Price = 42
            },
            new Tarif
            {
                Id = Identifier.Generate(),
                Name = Identifier.GenerateString(),
                Price = 42
            });

        var organisationContext = new Mock<IOrganizationContext>();
        organisationContext.Setup(x => x.Tarifs).Returns(database.Object.GetCollection<Tarif>());

        // Act
        var query = new TarifQuery();
        var result = query.GetTarifs(organisationContext.Object);

        // Assert
        result.Should().NotBeNull();
    }
}