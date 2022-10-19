// -----------------------------------------------------------------------
//  <copyright file = "PatientTypeTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using HotChocolate.Types;
using Moq;
using Prism.ProAssistant.Api.Graph.Patients;
using Prism.ProAssistant.Business.Models;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Graph.Patients;

public class PatientTypeTests
{
    [Fact]
    public void Configure_Ok()
    {
        // Arrange
        var descriptor = new Mock<IObjectTypeDescriptor<Patient>>();

        // Act
        PatientType.ConfigurePatient(descriptor.Object);

        // Assert
        descriptor.Invocations.Count.Should().Be(9);
    }
}