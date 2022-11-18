// -----------------------------------------------------------------------
//  <copyright file = "DocumentsController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Prism.ProAssistant.Api.Extensions;
using Prism.ProAssistant.Business.Commands;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Queries;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Documents.Generators;

namespace Prism.ProAssistant.Api.Controllers
{
    public class OldDocumentController : Controller
    {
        private readonly IDistributedCache _cache;
        private readonly IReceiptGenerator _receiptGenerator;

        public OldDocumentController(IReceiptGenerator receiptGenerator, IDistributedCache cache)
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

    public class DocumentsController : Controller
    {
        private readonly IMediator _mediator;

        public DocumentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("api/documents")]
        [HttpGet]
        public async Task<ActionResult<List<Document>>> FindMany()
        {
            var result = await _mediator.Send(new FindMany<Document>());
            return result
                .OrderBy(x => x.Name)
                .ToList()
                .ToActionResult();
        }

        [Route("api/documents/{documentId}")]
        [HttpGet]
        public async Task<ActionResult<Document>> FindOne(string documentId)
        {
            var result = await _mediator.Send(new FindOne<Document>(documentId));
            return result.ToActionResult();
        }

        [Route("api/documents/{documentId}")]
        [HttpDelete]
        public async Task RemoveOne(string documentId)
        {
            await _mediator.Send(new RemoveOne<Document>(documentId));
        }

        [Route("api/documents")]
        [HttpPost]
        public async Task<ActionResult<UpsertResult>> UpsertOne([FromBody] Document document)
        {
            var result = await _mediator.Send(new UpsertOne<Document>(document));
            return result.ToActionResult();
        }
    }
}