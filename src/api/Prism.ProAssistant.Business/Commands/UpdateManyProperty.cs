// -----------------------------------------------------------------------
//  <copyright file = "UpdateManyProperty.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Extensions;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Commands;

public record UpdateManyProperty(string OrganizationId, string UserId, string FilterProperty, string FilterValue, string Property, object Value);

public interface IUpdateManyPropertyHandler
{
    Task UpdateMany<T>(UpdateManyProperty request)
        where T : IDataModel;
}

public class UpdateManyPropertyHandler : IUpdateManyPropertyHandler
{
    private readonly ILogger<UpdateManyPropertyHandler> _logger;
    private readonly IOrganizationContext _organizationContext;

    public UpdateManyPropertyHandler(ILogger<UpdateManyPropertyHandler> logger, IOrganizationContext organizationContext)
    {
        _logger = logger;
        _organizationContext = organizationContext;
    }

    public async Task UpdateMany<T>(UpdateManyProperty request)
        where T : IDataModel
    {
        if (string.IsNullOrEmpty(request.FilterProperty))
        {
            _logger.LogWarning("Cannot update the property for type {itemType} and empty id", typeof(T).Name);
            return;
        }

        _organizationContext.SelectOrganization(request.OrganizationId);
        var collection = _organizationContext.GetCollection<T>();
        await _logger.LogPropertyManyUpdate<T>(request.UserId, request.Property, request.FilterProperty, async () =>
        {
            var result = await collection.UpdateManyAsync(Builders<T>.Filter.Eq(request.FilterProperty, request.FilterValue), Builders<T>.Update.Set(request.Property, request.Value));

            return result.ModifiedCount;
        });
    }
}