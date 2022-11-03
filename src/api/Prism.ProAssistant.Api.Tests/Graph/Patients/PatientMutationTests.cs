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
using Prism.ProAssistant.UnitTesting.Fakes;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Graph.Patients;

public class PatientMutationTests
{
    [Fact]
    public async Task CreatePatientAsync_Ok()
    {
        // Arrange
        var patientId = Identifier.GenerateString();

        var organisationContext = new OrganizationContextFake();

        // Act
        var query = new PatientMutation();
        var result = await query.CreatePatientAsync(new Patient
        {
            LastName = "Simon"
        }, organisationContext, Mock.Of<ILogger<PatientMutation>>(), Mock.Of<IUserContextAccessor>());

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBe(patientId);
    }

    [Fact]
    public async Task UpdatePatientAsync_Ok()
    {
        // Arrange
        var patientId = Identifier.GenerateString();
        var replacePatient = new Patient
        {
            Id = patientId,
            LastName = "Simon"
        };

        var organisationContext = new OrganizationContextFake
        {
            PatientsReplace = replacePatient
        };

        // Act
        var query = new PatientMutation();
        var result = await query.UpdatePatientAsync(replacePatient, organisationContext, Mock.Of<ILogger<PatientMutation>>(), Mock.Of<IUserContextAccessor>());

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(patientId);
    }
}