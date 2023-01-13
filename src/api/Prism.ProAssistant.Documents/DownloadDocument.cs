// -----------------------------------------------------------------------
//  <copyright file = "DownloadDocument.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Documents;

public record DownloadDocument(string DocumentId) : IRequest<DownloadDocumentResponse?>;

public record DownloadDocumentResponse(string FileName, byte[] FileContent);

public class DownloadDocumentHandler : IRequestHandler<DownloadDocument, DownloadDocumentResponse?>
{
    private readonly ILogger<DownloadDocumentHandler> _logger;
    private readonly IOrganizationContext _organizationContext;

    public DownloadDocumentHandler(IOrganizationContext organizationContext, ILogger<DownloadDocumentHandler> logger)
    {
        _organizationContext = organizationContext;
        _logger = logger;
    }

    public async Task<DownloadDocumentResponse?> Handle(DownloadDocument request, CancellationToken cancellationToken)
    {
        var bucket = _organizationContext.GetGridFsBucket();
        var file = await bucket.FindAsync(Builders<GridFSFileInfo>.Filter.Eq("_id", ObjectId.Parse(request.DocumentId)), cancellationToken: cancellationToken).Result
            .FirstOrDefaultAsync(cancellationToken);

        if (file == null)
        {
            _logger.LogWarning("File with id {itemId} not found", request.DocumentId);
            return null;
        }

        var bytes = await bucket.DownloadAsBytesByNameAsync(file.Filename, cancellationToken: cancellationToken);

        _logger.LogInformation("Start downloading file with id {itemId}", request.DocumentId);

        return new DownloadDocumentResponse(file.Filename, bytes);
    }
}