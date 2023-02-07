// -----------------------------------------------------------------------
//  <copyright file = "DownloadDocumentService.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Documents;

public record DownloadDocumentResponse(string FileName, byte[] FileContent);

public interface IDownloadDocumentService
{
    Task<DownloadDocumentResponse?> Download(string documentId);
}

public class DownloadDocumentService : IDownloadDocumentService
{
    private readonly ILogger<DownloadDocumentService> _logger;
    private readonly IOrganizationContext _organizationContext;

    public DownloadDocumentService(IOrganizationContext organizationContext, ILogger<DownloadDocumentService> logger)
    {
        _organizationContext = organizationContext;
        _logger = logger;
    }

    public async Task<DownloadDocumentResponse?> Download(string documentId)
    {
        var bucket = _organizationContext.GetGridFsBucket();
        var file = await bucket.FindAsync(Builders<GridFSFileInfo>.Filter.Eq("_id", ObjectId.Parse(documentId))).Result
            .FirstOrDefaultAsync();

        if (file == null)
        {
            _logger.LogWarning("File with id {itemId} not found", documentId);
            return null;
        }

        var bytes = await bucket.DownloadAsBytesByNameAsync(file.Filename);

        _logger.LogInformation("Start downloading file with id {itemId}", documentId);

        return new DownloadDocumentResponse(file.Filename, bytes);
    }
}