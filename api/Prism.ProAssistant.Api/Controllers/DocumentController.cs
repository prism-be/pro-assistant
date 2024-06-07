using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Prism.Core;
using Prism.Core.Exceptions;
using Prism.Infrastructure.Authentication;
using Prism.Infrastructure.Providers;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;
using Prism.ProAssistant.Domain.DayToDay.Appointments;
using Prism.ProAssistant.Domain.DayToDay.Appointments.Events;
using Prism.ProAssistant.Storage.Events;

namespace Prism.ProAssistant.Api.Controllers;

using Domain;
using Helpers;

public class DocumentController : Controller
{
    private readonly IDistributedCache _cache;
    private readonly IDataStorage _dataStorage;
    private readonly IEventStore _eventStore;
    private readonly IPdfService _pdfService;
    private readonly UserOrganization _userOrganization;

    public DocumentController(IDistributedCache cache, IEventStore eventStore, IPdfService pdfService, IDataStorage dataStorage, UserOrganization userOrganization)
    {
        _cache = cache;
        _eventStore = eventStore;
        _pdfService = pdfService;
        _dataStorage = dataStorage;
        _userOrganization = userOrganization;
    }

    [HttpDelete]
    [Route("api/document/{appointmentId}/{documentId}")]
    public async Task Delete(string appointmentId, string documentId)
    {
        ModelStateHelper.Validate(ModelState.IsValid);
        
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
        ModelStateHelper.Validate(ModelState.IsValid);
        
        var idBytes = await _cache.GetAsync(downloadKey);

        if (idBytes == null)
        {
            return NotFound();
        }

        var reference = JsonSerializer.Deserialize<FileReference>(Encoding.Default.GetString(idBytes));

        if (reference == null || id != reference.Id)
        {
            return BadRequest();
        }

        var stream = await _dataStorage.OpenFileStreamAsync(reference.Organization, "documents", id);
        var content = new FileStreamResult(stream, "application/pdf");

        if (download)
        {
            content.FileDownloadName = await _dataStorage.GetFileNameAsync(reference.Organization, "documents", id);
        }

        return content;
    }

    [HttpPost]
    [Route("api/document/download/{id}")]
    public async Task<DownloadReference> DownloadDocument(string id)
    {
        ModelStateHelper.Validate(ModelState.IsValid);
        
        if (await _dataStorage.ExistsAsync(_userOrganization.Organization, "documents", id) == false)
        {
            throw new NotFoundException("Document not found.");
        }

        var downloadKey = Identifier.GenerateString();
        var reference = new FileReference(id, _userOrganization.Organization);
        var data = Encoding.Default.GetBytes(JsonSerializer.Serialize(reference));

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
        ModelStateHelper.Validate(ModelState.IsValid);
        
        await _pdfService.GenerateDocument(request);
    }

    public record FileReference(string Id, string Organization);
}