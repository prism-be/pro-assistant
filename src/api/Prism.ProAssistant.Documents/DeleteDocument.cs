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

public record DeleteDocument(string Id, string AppointmentId) : IRequest;

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
        _logger.LogInformation("Delete Document {DocumentId} for Appointment {AppointmentId}", request.Id, request.AppointmentId);
        var appointments = _organizationContext.GetCollection<Appointment>();
        var appointment =
            await (await appointments.FindAsync(Builders<Appointment>.Filter.Eq(x => x.Id, request.AppointmentId), cancellationToken: cancellationToken)).SingleAsync(
                cancellationToken: cancellationToken);

        var deletedDocument = appointment.Documents.SingleOrDefault(x => x.Id == request.Id);

        if (deletedDocument == null)
        {
            _logger.LogWarning("Document {DocumentId} not found for Appointment {AppointmentId}", request.Id, request.AppointmentId);
            return Unit.Value;
        }

        var bucket = _organizationContext.GetGridFsBucket();
        await bucket.DeleteAsync(ObjectId.Parse(request.Id), cancellationToken);

        appointment.Documents.Remove(deletedDocument);
        await appointments.UpdateOneAsync(Builders<Appointment>.Filter.Eq(x => x.Id, request.AppointmentId), Builders<Appointment>.Update.Set(x => x.Documents, appointment.Documents),
            cancellationToken: cancellationToken);

        return Unit.Value;
    }
}