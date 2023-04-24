using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Prism.Core;
using Prism.Core.Exceptions;
using Prism.Infrastructure.Providers;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;
using Prism.ProAssistant.Domain.DayToDay.Appointments;
using Prism.ProAssistant.Domain.DayToDay.Appointments.Events;
using Prism.ProAssistant.Storage.Events;

namespace Prism.ProAssistant.Api.Controllers;

public class DocumentController : Controller
{
    private readonly IDistributedCache _cache;
    private readonly IDataStorage _dataStorage;
    private readonly IEventStore _eventStore;
    private readonly IPdfService _pdfService;

    public DocumentController(IDistributedCache cache, IEventStore eventStore, IPdfService pdfService, IDataStorage dataStorage)
    {
        _cache = cache;
        _eventStore = eventStore;
        _pdfService = pdfService;
        _dataStorage = dataStorage;
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
        var idBytes = await _cache.GetAsync(downloadKey);

        if (idBytes == null)
        {
            return NotFound();
        }

        var fileId = Encoding.Default.GetString(idBytes);

        if (id != fileId)
        {
            return BadRequest();
        }

        var stream = await _dataStorage.OpenFileStreamAsync("documents", id);
        var content = new FileStreamResult(stream, "application/pdf");

        if (download)
        {
            content.FileDownloadName = await _dataStorage.GetFileNameAsync("documents", id);
        }

        return content;
    }

    [HttpPost]
    [Route("api/document/download/{id}")]
    public async Task<DownloadReference> DownloadDocument(string id)
    {
        if (await _dataStorage.ExistsAsync("documents", id) == false)
        {
            throw new NotFoundException("Document not found.");
        }

        var downloadKey = Identifier.GenerateString();
        var data = Encoding.Default.GetBytes(id);

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