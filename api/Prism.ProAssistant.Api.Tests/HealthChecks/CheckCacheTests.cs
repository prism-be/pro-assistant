using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using Prism.ProAssistant.Api.Healtchecks;

namespace Prism.ProAssistant.Api.Tests.HealthChecks;

public class CheckCacheTests
{
    [Fact]
    public async Task Check_Healthy()
    {
        // Arrange
        var cache = new Mock<IDistributedCache>();

        // Act
        var check = new CheckCache(cache.Object);
        var result = await check.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

        // Assert
        result.Status.Should().Be(HealthStatus.Healthy);
    }

    [Fact]
    public async Task Check_Unhealthy()
    {
        // Arrange
        var cache = new Mock<IDistributedCache>();
        cache.Setup(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws<NotSupportedException>();

        // Act
        var check = new CheckCache(cache.Object);
        var result = await check.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

        // Assert
        result.Status.Should().Be(HealthStatus.Unhealthy);
    }
}