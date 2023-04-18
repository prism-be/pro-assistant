using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Prism.Core.Exceptions;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;
using Prism.ProAssistant.Domain.DayToDay.Appointments;
using Prism.ProAssistant.Domain.DayToDay.Appointments.Events;
using Prism.ProAssistant.Storage;
using Prism.ProAssistant.Storage.Events;

namespace Prism.ProAssistant.Api.Controllers;

public class DocumentController : Controller
{
    private readonly IDistributedCache _cache;
    private readonly IQueryService _queryService;
    private readonly IEventStore _eventStore;
    private readonly IPdfService _pdfService;
    private readonly IFileService _fileService;

    public DocumentController(IDistributedCache cache, IQueryService queryService, IEventStore eventStore, IPdfService pdfService, IFileService fileService)
    {
        _cache = cache;
        _queryService = queryService;
        _eventStore = eventStore;
        _pdfService = pdfService;
        _fileService = fileService;
    }

    [HttpDelete]
    [Route("api/document/{appointmentId}/{documentId}")]
    public async Task Delete(string appointmentId, string documentId)
    {
        await _eventStore.RaiseAndPersist<Appointment>(new DetachAppointmentDocument
        {
            StreamId = appointmentId,
            DocumentId = documentId
        });
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
            content.FileDownloadName = await _fileService.GetFileNameAsync(id);
        }

        return content;
    }

    [HttpPost]
    [Route("api/document/download/{id}")]
    public async Task<DownloadReference> DownloadDocument(string id)
    {
        var data = await _fileService.GetFileAsync(id);

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