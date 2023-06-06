namespace Prism.ProAssistant.Api.Controllers.Data.Accounting;

using Domain.DayToDay.Appointments;
using Infrastructure.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storage;

[Authorize]
public class ClosingController: Controller
{
    private readonly IQueryService _queryService;

    public ClosingController(IQueryService queryService)
    {
        _queryService = queryService;
    }
    
    [HttpGet]
    [Route("api/data/accounting/closing/unclosed")]
    public async Task<IEnumerable<Appointment>> ListUnclosed()
    {
        return await _queryService.SearchAsync<Appointment>(
            new Filter(nameof(Appointment.Payment), (int)PaymentTypes.Unpayed),
            new Filter(nameof(Appointment.State), (int)AppointmentState.Canceled, FilterOperator.NotEqual)
            );
    }
}