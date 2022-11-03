// -----------------------------------------------------------------------
//  <copyright file = "DocumentsController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Documents.Generators;

namespace Prism.ProAssistant.Api.Controllers;

public class DocumentsController: Controller
{
    private readonly IReceiptGenerator _receiptGenerator;

    public DocumentsController(IReceiptGenerator receiptGenerator)
    {
        _receiptGenerator = receiptGenerator;
    }

    [HttpGet]
    [Route("api/documents/receipt/{meetingId}")]
    public async Task<IActionResult> Receipt(string meetingId)
    {
        var pdfBytes = await _receiptGenerator.Generate(meetingId);

        if (pdfBytes == null)
        {
            return this.NotFound();
        }

        var stream = new MemoryStream(pdfBytes);
        return this.File(stream, "application/pdf", $"receipt-{meetingId}.pdf");
    }
}