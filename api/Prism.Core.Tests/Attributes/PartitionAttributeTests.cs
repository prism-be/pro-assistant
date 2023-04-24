using FluentAssertions;
using Prism.Core.Attributes;
using Prism.Core.Exceptions;
using CollectionAttribute = Prism.Core.Attributes.CollectionAttribute;

namespace Prism.Core.Tests.Attributes;

public class PartitionAttributeTests
{

    [Fact]
    public void GetPartition_Ko()
    {
        // Act
        var partition = PartitionAttribute.GetPartition<Bar>();

        // Assert
        partition.Should().Be("Id");
    }

    [Fact]
    public void GetPartition_Ok()
    {
        // Act
        var partition = PartitionAttribute.GetPartition<Foo>();

        // Assert
        partition.Should().Be("foo");
    }

    [Partition("foo")]
    private class Foo
    {
    }

    private class Bar
    {
    }
}