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
using Prism.ProAssistant.Api.Graph.Tariffs;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;
using Prism.ProAssistant.UnitTesting;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Graph.Tariffs;

public class TariffQueryTests
{
    [Fact]
    public async Task GetTariffById_Ok()
    {
        // Arrange
        var tarifId = Identifier.GenerateString();
        var database = new Mock<IMongoDatabase>();
        database.SetupCollection(new Tariff
        {
            Id = tarifId,
            Name = Identifier.GenerateString(),
            Price = 42
        });

        var organisationContext = new Mock<IOrganizationContext>();
        organisationContext.Setup(x => x.Tariffs).Returns(database.Object.GetCollection<Tariff>());

        // Act
        var query = new TariffQuery();
        var result = query.GetTariffById(tarifId, organisationContext.Object);
        var patient = await result.SingleOrDefaultAsync(CancellationToken.None) as Tariff;

        // Assert
        patient.Should().NotBeNull();
        patient?.Id.Should().Be(tarifId);
    }

    [Fact]
    public void GetTariffs_Ok()
    {
        // Arrange
        var database = new Mock<IMongoDatabase>();
        database.SetupCollection(new Tariff
            {
                Id = Identifier.GenerateString(),
                Name = Identifier.GenerateString(),
                Price = 42
            },
            new Tariff
            {
                Id = Identifier.GenerateString(),
                Name = Identifier.GenerateString(),
                Price = 42
            });

        var organisationContext = new Mock<IOrganizationContext>();
        organisationContext.Setup(x => x.Tariffs).Returns(database.Object.GetCollection<Tariff>());

        // Act
        var query = new TariffQuery();
        var result = query.GetTariffs(organisationContext.Object);

        // Assert
        result.Should().NotBeNull();
    }
}