namespace Prism.ProAssistant.Api.Controllers.Data.Accounting;

using Domain.Accounting.Reporting;
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
    [Route("api/data/accounting/reporting/periods")]
    public async Task<IEnumerable<AccountingReportingPeriod>> ListPeriods()
    {
        return await _queryService.ListAsync<AccountingReportingPeriod>();
    }
}