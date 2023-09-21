namespace Prism.ProAssistant.Api.Tests.Controllers.Accounting;

using Api.Controllers.Data.Accounting;
using Domain.Accounting.Reporting;
using Infrastructure.Providers;
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
        await reportingController.ListPeriods(2023);

        // Assert
        queryService.Verify(x => x.SearchAsync<AccountingReportingPeriod>(It.Is<Filter[]>(p => p.Length == 1)), Times.Once);
    }
}