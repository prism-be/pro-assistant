// -----------------------------------------------------------------------
//  <copyright file = "TarifTypeTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using HotChocolate.Types;
using Moq;
using Prism.ProAssistant.Api.Graph.Tariffs;
using Prism.ProAssistant.Business.Models;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Graph.Tariffs;

public class TariffTypeTests
{
    [Fact]
    public void Configure_Ok()
    {
        // Arrange
        var descriptor = new Mock<IObjectTypeDescriptor<Tariff>>();

        // Act
        TariffType.ConfigureTariff(descriptor.Object);

        // Assert
        descriptor.Invocations.Count.Should().Be(4);
    }
}