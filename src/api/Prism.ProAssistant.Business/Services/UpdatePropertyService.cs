// -----------------------------------------------------------------------
//  <copyright file = "UpdatePropertyService.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Events;
using Prism.ProAssistant.Business.Extensions;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Services;

public interface IUpdatePropertyService
{
    Task Update<T>(string id, string property, object value)
        where T : IDataModel;
}

public class UpdatePropertyService : IUpdatePropertyService
{
    private readonly ILogger<UpdatePropertyService> _logger;
    private readonly IOrganizationContext _organizationContext;
    private readonly IPublisher _publisher;
    private readonly User _user;

    public UpdatePropertyService(ILogger<UpdatePropertyService> logger, IOrganizationContext organizationContext, IPublisher publisher, User user)
    {
        _logger = logger;
        _organizationContext = organizationContext;
        _publisher = publisher;
        _user = user;
    }

    public async Task Update<T>(string id, string property, object value)
        where T : IDataModel
    {
        if (string.IsNullOrEmpty(id))
        {
            _logger.LogWarning("Cannot update the property for type {itemType} and empty id", typeof(T).Name);
            return;
        }

        var collection = _organizationContext.GetCollection<T>();
        await _logger.LogPropertyUpdate<T>(_user, property, id, async () =>
        {
            var result = await collection.UpdateOneAsync(Builders<T>.Filter.Eq("Id", id), Builders<T>.Update.Set(property, value));

            if (result.ModifiedCount > 0)
            {
                _publisher.Publish(Topics.PropertyUpdated, new PropertyUpdated(typeof(T).Name, id, property, value));
            }

            return id;
        });
    }
}