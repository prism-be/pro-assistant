using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Moq;
using Prism.ProAssistant.Api.Insights;

namespace Prism.ProAssistant.Api.Tests.Insights;

public class CleanTelemetryFilterTests
{

    [Fact]
    public void FilterHealth_FilterHealthChecks()
    {
        // Arrange
        var next = new Mock<ITelemetryProcessor>();

        // Act
        var filter = new CleanTelemetryFilter(next.Object);
        filter.Process(new RequestTelemetry { Url = new Uri("http://localhost/api/health") });

        // Assert
        next.Verify(x => x.Process(It.IsAny<ITelemetry>()), Times.Never);
    }

    [Fact]
    public void FilterHealth_Ok()
    {
        // Arrange
        var next = new Mock<ITelemetryProcessor>();

        // Act
        var filter = new CleanTelemetryFilter(next.Object);
        filter.Process(new RequestTelemetry { Url = new Uri("http://localhost/") });

        // Assert
        next.Verify(x => x.Process(It.IsAny<ITelemetry>()), Times.Once);
    }
}