// -----------------------------------------------------------------------
//  <copyright file = "DownloadController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

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
    public async Task<IActionResult> Download(string downloadKey)
    {
        var pdfBytes = await _cache.GetAsync(GenerateDownloadKey(downloadKey));

        if (pdfBytes == null)
        {
            return NotFound();
        }

        var stream = new MemoryStream(pdfBytes);

        return File(stream, "application/pdf");
    }

    [HttpPost]
    [Route("api/document/generate")]
    public async Task Generate([FromBody]GenerateDocument request)
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