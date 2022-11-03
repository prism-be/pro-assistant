// -----------------------------------------------------------------------
//  <copyright file = "LocalizatorTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Prism.ProAssistant.Documents.Locales;
using Xunit;

namespace Prism.ProAssistant.Documents.Tests.Locales;

public class LocalizatorTests
{

    [Fact]
    public void GetLocale_DontDoubleLoad()
    {
        // Arrange
        var localizator = new Localizator();

        // Act
        localizator.GetTranslation("receipt", "title");
        var title = localizator.GetTranslation("receipt", "title");

        // Assert
        title.Should().Be("Reçu de paiement");
    }

    [Fact]
    public void GetLocale_Ok()
    {
        // Arrange
        var localizator = new Localizator();

        // Act
        var title = localizator.GetTranslation("receipt", "title");

        // Assert
        title.Should().Be("Reçu de paiement");
    }
}