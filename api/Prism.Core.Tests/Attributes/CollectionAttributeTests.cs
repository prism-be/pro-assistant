using FluentAssertions;
using Prism.Core.Exceptions;
using CollectionAttribute = Prism.Core.Attributes.CollectionAttribute;

namespace Prism.Core.Tests.Attributes;

public class CollectionAttributeTests
{

    [Fact]
    public void GetCollectionName_Ko()
    {
        // Act
        Action action = () => CollectionAttribute.GetCollectionName<Bar>();

        // Assert
        action.Should().Throw<MissingConfigurationException>();
    }

    [Fact]
    public void GetCollectionName_Ok()
    {
        // Act
        var name = CollectionAttribute.GetCollectionName<Foo>();

        // Assert
        name.Should().Be("foo");
    }

    [Collection("foo")]
    private class Foo
    {
    }

    private class Bar
    {
    }
}