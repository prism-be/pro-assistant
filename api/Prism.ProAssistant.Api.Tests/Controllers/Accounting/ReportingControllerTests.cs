namespace Prism.ProAssistant.Api.Tests.Controllers.Accounting;

using Api.Controllers.Data.Accounting;
using Domain.Accounting.Reporting;
using Moq;
using Storage;

public class ReportingControllerTests
{
    [Fact]
    public async Task ListPeriods_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        
        // Act
        var reportingController = new ReportingController(queryService.Object);
        await reportingController.ListPeriods();

        // Assert
        queryService.Verify(x => x.ListAsync<AccountingReportingPeriod>(), Times.Once);
    }
}