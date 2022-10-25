// -----------------------------------------------------------------------
//  <copyright file = "PatientMutationTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Api.Graph.Patients;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Graph.Patients;

public class PatientMutationTests
{
    [Fact]
    public async Task CreatePatientAsync_Ok()
    {
        // Arrange
        var patientId = Identifier.Generate();
        var database = new Mock<IMongoDatabase>();
        database.SetupCollection(new Patient
        {
            Id = Identifier.Generate(),
            LastName = "Baudart"
        });

        var organisationContext = new Mock<IOrganizationContext>();
        organisationContext.Setup(x => x.Patients).Returns(database.Object.GetCollection<Patient>());

        // Act
        var query = new PatientMutation();
        var result = await query.CreatePatientAsync(new Patient
        {
            Id = patientId,
            LastName = "Simon"
        }, organisationContext.Object, Mock.Of<ILogger<PatientMutation>>());

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBe(patientId);
    }

    [Fact]
    public async Task UpdatePatientAsync_Ok()
    {
        // Arrange
        var patientId = Identifier.Generate();
        var replacePatient = new Patient
        {
            Id = patientId,
            LastName = "Simon"
        };
        
        var database = new Mock<IMongoDatabase>();
        database.SetupCollectionAndReplace(replacePatient, new Patient
        {
            Id = patientId,
            LastName = "Baudart"
        });

        var organisationContext = new Mock<IOrganizationContext>();
        organisationContext.Setup(x => x.Patients).Returns(database.Object.GetCollection<Patient>());

        // Act
        var query = new PatientMutation();
        var result = await query.UpdatePatientAsync(replacePatient, organisationContext.Object, Mock.Of<ILogger<PatientMutation>>());

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(patientId);
    }
}