// -----------------------------------------------------------------------
//  <copyright file = "DocumentsController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Documents.Generators;

namespace Prism.ProAssistant.Api.Controllers;

public class DocumentsController : Controller
{
    private readonly IDistributedCache _cache;
    private readonly IReceiptGenerator _receiptGenerator;

    public DocumentsController(IReceiptGenerator receiptGenerator, IDistributedCache cache)
    {
        _receiptGenerator = receiptGenerator;
        _cache = cache;
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("api/documents/receipt/{meetingKey}")]
    public async Task<IActionResult> Receipt(string meetingKey)
    {
        var pdfBytes = await _cache.GetAsync(GenerateMeetingKey(meetingKey));

        if (pdfBytes == null)
        {
            return NotFound();
        }

        var stream = new MemoryStream(pdfBytes);

        return File(stream, "application/pdf");
    }

    [HttpPost]
    [Route("api/documents/receipt/{meetingId}")]
    public async Task<IActionResult> StartReceipt(string meetingId)
    {
        var meetingKey = Identifier.GenerateString();
        var pdfBytes = await _receiptGenerator.Generate(meetingId);
        await _cache.SetAsync(GenerateMeetingKey(meetingKey), pdfBytes, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });

        return Ok(new
        {
            key = meetingKey
        });
    }

    private static string GenerateMeetingKey(string meetingKey)
    {
        return $"documents/receipt/{meetingKey}";
    }
}