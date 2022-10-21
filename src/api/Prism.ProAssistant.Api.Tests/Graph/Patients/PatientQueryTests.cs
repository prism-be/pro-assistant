// -----------------------------------------------------------------------
//  <copyright file = "PatientQueryTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Api.Graph.Patients;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Graph.Patients;

public class PatientQueryTests
{
    [Fact]
    public async Task GetPatientById_Ok()
    {
        // Arrange
        var patientId = Identifier.Generate();
        var database = new Mock<IMongoDatabase>();
        database.SetupCollection(new Patient
        {
            Id = patientId,
            LastName = "Baudart"
        });

        var organisationContext = new Mock<IOrganizationContext>();
        organisationContext.Setup(x => x.Patients).Returns(database.Object.GetCollection<Patient>());

        // Act
        var query = new PatientQuery();
        var result = query.GetPatientById(patientId, organisationContext.Object);
        var patient = await result.SingleOrDefaultAsync(CancellationToken.None) as Patient;

        // Assert
        patient.Should().NotBeNull();
        patient?.Id.Should().Be(patientId);
    }

    [Fact]
    public void GetPatients_Ok()
    {
        // Arrange
        var database = new Mock<IMongoDatabase>();
        database.SetupCollection(new Patient
            {
                Id = Identifier.Generate(),
                LastName = "Baudart"
            },
            new Patient
            {
                Id = Identifier.Generate(),
                LastName = "Simon"
            });

        var organisationContext = new Mock<IOrganizationContext>();
        organisationContext.Setup(x => x.Patients).Returns(database.Object.GetCollection<Patient>());

        // Act
        var query = new PatientQuery();
        var result = query.GetPatients(organisationContext.Object);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void SearchPatients_EmptyFilters()
    {
        // Arrange
        var database = new Mock<IMongoDatabase>();
        database.SetupCollection(new Patient
            {
                Id = Identifier.Generate(),
                LastName = "Baudart",
                FirstName = "Simon",
                BirthDate = "14/05",
                PhoneNumber = "123456789"
            },
            new Patient
            {
                Id = Identifier.Generate(),
                LastName = "Simon"
            });

        var organisationContext = new Mock<IOrganizationContext>();
        organisationContext.Setup(x => x.Patients).Returns(database.Object.GetCollection<Patient>());

        // Act
        var query = new PatientQuery();
        var result = query.SearchPatients(string.Empty, string.Empty, string.Empty, string.Empty, organisationContext.Object);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void SearchPatients_Ok()
    {
        // Arrange
        var database = new Mock<IMongoDatabase>();
        database.SetupCollection(new Patient
            {
                Id = Identifier.Generate(),
                LastName = "Baudart",
                FirstName = "Simon",
                BirthDate = "14/05",
                PhoneNumber = "123456789"
            },
            new Patient
            {
                Id = Identifier.Generate(),
                LastName = "Simon"
            });

        var organisationContext = new Mock<IOrganizationContext>();
        organisationContext.Setup(x => x.Patients).Returns(database.Object.GetCollection<Patient>());

        // Act
        var query = new PatientQuery();
        var result = query.SearchPatients("bau", "sim", "123", "14", organisationContext.Object);

        // Assert
        result.Should().NotBeNull();
    }
}