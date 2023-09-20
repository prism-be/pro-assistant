namespace Prism.ProAssistant.Api.Controllers.Data.Accounting;

using Domain.Accounting.Reporting;
using Infrastructure.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storage;

[Authorize]
public class ReportingController : Controller
{
    private readonly IQueryService _queryService;

    public ReportingController(IQueryService queryService)
    {
        _queryService = queryService;
    }

    [HttpGet]
    [Route("api/data/accounting/reporting/periods/{year:int}")]
    public async Task<IEnumerable<AccountingReportingPeriod>> ListPeriods(int year)
    {
        var startOfYear = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var data = await _queryService.SearchAsync<AccountingReportingPeriod>(new Filter(nameof(AccountingReportingPeriod.StartDate), startOfYear, FilterOperator.GreaterThanOrEqual));
        return data.Where(x => x.StartDate.Year == year).OrderBy(x => x.StartDate);
    }
}