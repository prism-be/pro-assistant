// -----------------------------------------------------------------------
//  <copyright file = "UpsertOneService.cs" company = "Prism">
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

public interface IUpsertOneService
{
    Task<UpsertResult> Upsert<T>(T item)
        where T : IDataModel;
}

public class UpsertOneService : IUpsertOneService
{
    private readonly ILogger<UpsertOneService> _logger;

    private readonly IOrganizationContext _organizationContext;

    private readonly IPropertyUpdatePublisher _publisher;
    private readonly User _user;

    public UpsertOneService(ILogger<UpsertOneService> logger, IOrganizationContext organizationContext, User user, IPropertyUpdatePublisher publisher)
    {
        _organizationContext = organizationContext;
        _user = user;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<UpsertResult> Upsert<T>(T item)
        where T : IDataModel
    {
        var collection = _organizationContext.GetCollection<T>();

        if (string.IsNullOrWhiteSpace(item.Id))
        {
            var result = await _logger.LogDataInsert(_user, item, async () =>
            {
                await collection.InsertOneAsync(item);
                return new UpsertResult(item.Id, _user.Organization);
            });

            PublishPropertyUpdate(item);

            return result;
        }

        return await _logger.LogDataUpdate(_user, item, async () =>
        {
            var updated = await collection.FindOneAndReplaceAsync(Builders<T>.Filter.Eq("Id", item.Id), item, new FindOneAndReplaceOptions<T>
            {
                IsUpsert = true
            });

            PublishPropertyUpdate(item);

            return new UpsertResult(updated.Id, _user.Organization);
        });
    }

    private void PublishPropertyUpdate<T>(T item) where T : IDataModel
    {
        Task.Run(() =>
        {
            foreach (var property in item.GetType().GetProperties())
            {
                var value = property.GetValue(item);

                if (value != null)
                {
                    _publisher.Publish(new PropertyUpdated(typeof(T).Name, item.Id, property.Name, value));
                }
            }
        });
    }
}