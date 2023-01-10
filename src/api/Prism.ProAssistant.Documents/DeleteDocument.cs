// -----------------------------------------------------------------------
//  <copyright file = "DeleteDocument.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Extensions;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Documents;

public record DeleteDocument(string Id, string AppointmentId) : IRequest;

public class DeleteDocumentHandler : IRequestHandler<DeleteDocument>
{
    private readonly ILogger<DeleteDocumentHandler> _logger;
    private readonly IOrganizationContext _organizationContext;
    private readonly IUserContextAccessor _userContextAccessor;

    public DeleteDocumentHandler(ILogger<DeleteDocumentHandler> logger, IOrganizationContext organizationContext, IUserContextAccessor userContextAccessor)
    {
        _logger = logger;
        _organizationContext = organizationContext;
        _userContextAccessor = userContextAccessor;
    }

    public async Task<Unit> Handle(DeleteDocument request, CancellationToken cancellationToken)
    {
        await _logger.LogDataDelete(_userContextAccessor, request.Id, async () =>
        {
            _logger.LogInformation("Delete Document {itemId} for Appointment {appointmentId}", request.Id, request.AppointmentId);
            var appointments = _organizationContext.GetCollection<Appointment>();
            var appointment =
                await (await appointments.FindAsync(Builders<Appointment>.Filter.Eq(x => x.Id, request.AppointmentId), cancellationToken: cancellationToken)).SingleAsync(
                    cancellationToken: cancellationToken);

            var deletedDocument = appointment.Documents.SingleOrDefault(x => x.Id == request.Id);

            if (deletedDocument == null)
            {
                _logger.LogWarning("Document {itemId} not found for Appointment {appointmentId}", request.Id, request.AppointmentId);
                return;
            }

            var bucket = _organizationContext.GetGridFsBucket();
            await bucket.DeleteAsync(ObjectId.Parse(request.Id), cancellationToken);

            appointment.Documents.Remove(deletedDocument);
            await appointments.UpdateOneAsync(Builders<Appointment>.Filter.Eq(x => x.Id, request.AppointmentId), Builders<Appointment>.Update.Set(x => x.Documents, appointment.Documents),
                cancellationToken: cancellationToken);

        });
        
        return Unit.Value;
    }
}