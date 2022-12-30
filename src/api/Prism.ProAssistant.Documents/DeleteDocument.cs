// -----------------------------------------------------------------------
//  <copyright file = "DeleteDocument.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Documents;

public record DeleteDocument(string Id, string MeetingId) : IRequest;

public class DeleteDocumentHandler : IRequestHandler<DeleteDocument>
{
    private readonly ILogger<DeleteDocumentHandler> _logger;
    private readonly IOrganizationContext _organizationContext;

    public DeleteDocumentHandler(ILogger<DeleteDocumentHandler> logger, IOrganizationContext organizationContext)
    {
        _logger = logger;
        _organizationContext = organizationContext;
    }

    public async Task<Unit> Handle(DeleteDocument request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete Document {DocumentId} for Meeting {MeetingId}", request.Id, request.MeetingId);
        var meetings = _organizationContext.GetCollection<Meeting>();
        var meeting =
            await (await meetings.FindAsync(Builders<Meeting>.Filter.Eq(x => x.Id, request.MeetingId), cancellationToken: cancellationToken)).SingleAsync(
                cancellationToken: cancellationToken);

        var deletedDocument = meeting.Documents.SingleOrDefault(x => x.Id == request.Id);

        if (deletedDocument == null)
        {
            _logger.LogWarning("Document {DocumentId} not found for Meeting {MeetingId}", request.Id, request.MeetingId);
            return Unit.Value;
        }

        var bucket = _organizationContext.GetGridFsBucket();
        await bucket.DeleteAsync(ObjectId.Parse(request.Id), cancellationToken);

        meeting.Documents.Remove(deletedDocument);
        await meetings.UpdateOneAsync(Builders<Meeting>.Filter.Eq(x => x.Id, request.MeetingId), Builders<Meeting>.Update.Set(x => x.Documents, meeting.Documents),
            cancellationToken: cancellationToken);

        return Unit.Value;
    }
}