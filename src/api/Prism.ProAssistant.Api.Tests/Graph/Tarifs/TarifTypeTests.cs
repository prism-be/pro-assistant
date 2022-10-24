// -----------------------------------------------------------------------
//  <copyright file = "TarifTypeTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using HotChocolate.Types;
using Moq;
using Prism.ProAssistant.Api.Graph.Tarifs;
using Prism.ProAssistant.Business.Models;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Graph.Tarifs;

public class TarifTypeTests
{
    [Fact]
    public void Configure_Ok()
    {
        // Arrange
        var descriptor = new Mock<IObjectTypeDescriptor<Tarif>>();

        // Act
        TarifType.ConfigureTarif(descriptor.Object);

        // Assert
        descriptor.Invocations.Count.Should().Be(3);
    }
}