// -----------------------------------------------------------------------
//  <copyright file = "DownloadController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text;
using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Documents;

namespace Prism.ProAssistant.Api.Controllers;

public class DownloadController : Controller
{
    private readonly IDistributedCache _cache;
    private readonly IMediator _mediator;

    public DownloadController(IMediator mediator, IDistributedCache cache)
    {
        _mediator = mediator;
        _cache = cache;
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("api/downloads/{downloadKey}")]
    public async Task<IActionResult> Download(string downloadKey, [FromQuery] bool download)
    {
        var pdfBytes = await _cache.GetAsync(GenerateDownloadKey(downloadKey));

        if (pdfBytes == null)
        {
            return NotFound();
        }

        var document = JsonSerializer.Deserialize<DownloadDocumentResponse>(pdfBytes);
        
        if (document == null)
        {
            return NotFound();
        }

        var content = new FileContentResult(document.FileContent, "application/pdf");

        if (download)
        {
            content.FileDownloadName = document.FileName;
        }

        return content;
    }

    [HttpPost]
    [Route("api/downloads/start")]
    public async Task<ActionResult<DownloadKey>> Generate([FromBody] DownloadDocument request)
    {
        var downloadKey = Identifier.GenerateString();
        var document = await _mediator.Send(request);

        if (document == null)
        {
            return NotFound();
        }

        var data = Encoding.Default.GetBytes(JsonSerializer.Serialize(document));
        await _cache.SetAsync(GenerateDownloadKey(downloadKey), data, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });

        return new DownloadKey(downloadKey);
    }

    [HttpPost]
    [Route("api/document/generate")]
    public async Task Generate([FromBody] GenerateDocument request)
    {
        var downloadKey = Identifier.GenerateString();
        var pdfBytes = await _mediator.Send(request);

        await _cache.SetAsync(GenerateDownloadKey(downloadKey), pdfBytes, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });
    }

    private static string GenerateDownloadKey(string downloadKey)
    {
        return $"downloads/{downloadKey}";
    }
}