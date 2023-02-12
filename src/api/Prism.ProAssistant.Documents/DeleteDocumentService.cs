// -----------------------------------------------------------------------
//  <copyright file = "DeleteDocumentService.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Extensions;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Documents;

public interface IDeleteDocumentService
{
    Task Delete(string id, string appointmentId);
}

public class DeleteDocumentService : IDeleteDocumentService
{
    private readonly ILogger<DeleteDocumentService> _logger;
    private readonly IOrganizationContext _organizationContext;
    private readonly User _user;

    public DeleteDocumentService(ILogger<DeleteDocumentService> logger, IOrganizationContext organizationContext, User user)
    {
        _logger = logger;
        _organizationContext = organizationContext;
        _user = user;
    }

    public async Task Delete(string id, string appointmentId)
    {
        await _logger.LogDataDelete(_user, id, async () =>
        {
            _logger.LogInformation("Delete Document {itemId} for Appointment {appointmentId}", id, appointmentId);
            var appointments = _organizationContext.GetCollection<Appointment>();
            var appointment =
                await (await appointments.FindAsync(Builders<Appointment>.Filter.Eq(x => x.Id, appointmentId))).SingleAsync();

            var deletedDocument = appointment.Documents.SingleOrDefault(x => x.Id == id);

            if (deletedDocument == null)
            {
                _logger.LogWarning("Document {itemId} not found for Appointment {appointmentId}", id, appointmentId);
                return;
            }

            var bucket = _organizationContext.GetGridFsBucket();
            await bucket.DeleteAsync(ObjectId.Parse(id));

            appointment.Documents.Remove(deletedDocument);
            await appointments.UpdateOneAsync(Builders<Appointment>.Filter.Eq(x => x.Id, appointmentId), Builders<Appointment>.Update.Set(x => x.Documents, appointment.Documents));
        });
    }
}