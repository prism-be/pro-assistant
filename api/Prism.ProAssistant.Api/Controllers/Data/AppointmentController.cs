using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;

namespace Prism.ProAssistant.Api.Controllers.Data;

[Authorize]
public class AppointmentController : Controller
{
    private readonly IDataService _dataService;

    public AppointmentController(IDataService dataService)
    {
        _dataService = dataService;
    }

    [HttpPost]
    [Route("api/data/appointments/insert")]
    public async Task<UpsertResult> Insert([FromBody] Appointment request)
    {
        return await _dataService.InsertAsync(request);
    }

    [HttpGet]
    [Route("api/data/appointments")]
    public async Task<List<Appointment>> List()
    {
        return await _dataService.ListAsync<Appointment>();
    }

    [HttpPost]
    [Route("api/data/appointments/search")]
    public async Task<List<Appointment>> Search([FromBody] List<SearchFilter> request)
    {
        return await _dataService.SearchAsync<Appointment>(request);
    }

    [HttpPost]
    [Route("api/data/appointments/update")]
    public async Task<UpsertResult> Search([FromBody] Appointment request)
    {
        return await _dataService.UpdateAsync(request);
    }

    [HttpGet]
    [Route("api/data/appointments/{id}")]
    public async Task<Appointment> Single(string id)
    {
        return await _dataService.SingleAsync<Appointment>(id);
    }
}