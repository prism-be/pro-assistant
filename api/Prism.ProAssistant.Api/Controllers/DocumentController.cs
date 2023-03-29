using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Prism.ProAssistant.Api.Exceptions;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;

namespace Prism.ProAssistant.Api.Controllers;

public class DocumentController : Controller
{
    private readonly IDistributedCache _cache;
    private readonly IDataService _dataService;
    private readonly IEventService _eventService;
    private readonly IPdfService _pdfService;

    public DocumentController(IPdfService pdfService, IDataService dataService, IDistributedCache cache, IEventService eventService)
    {
        _pdfService = pdfService;
        _dataService = dataService;
        _cache = cache;
        _eventService = eventService;
    }

    [HttpDelete]
    [Route("api/document/{appointmentId}/{documentId}")]
    public async Task Delete(string appointmentId, string documentId)
    {
        var appointment = await _dataService.SingleAsync<Appointment>(appointmentId);

        appointment.Documents.Remove(appointment.Documents.Single(x => x.Id == documentId));
        await _eventService.UpdateAsync<Appointment>(appointment.Id, new KeyValuePair<string, object>(nameof(appointment.Documents), appointment.Documents));

        await _dataService.DeleteFileAsync(documentId);
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("api/document/download/{id}/{downloadKey}")]
    public async Task<IActionResult> Download(string id, string downloadKey, [FromQuery] bool download)
    {
        var pdfBytes = await _cache.GetAsync(downloadKey);

        if (pdfBytes == null)
        {
            return NotFound();
        }

        var content = new FileContentResult(pdfBytes, "application/pdf");

        if (download)
        {
            content.FileDownloadName = await _dataService.GetFileNameAsync(id);
        }

        return content;
    }

    [HttpPost]
    [Route("api/document/download/{id}")]
    public async Task<DownloadReference> DownloadDocument(string id)
    {
        var data = await _dataService.GetFileAsync(id);

        if (data == null)
        {
            throw new NotFoundException("Document not found.");
        }

        var downloadKey = Guid.NewGuid().ToString();

        await _cache.SetAsync(downloadKey, data, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });

        return new DownloadReference
        {
            Id = downloadKey
        };
    }

    [HttpPost]
    [Route("api/document/generate")]
    public async Task GenerateDocument([FromBody] DocumentRequest request)
    {
        await _pdfService.GenerateDocument(request);
    }
}