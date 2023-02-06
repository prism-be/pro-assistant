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

public interface IUpdateManyPropertyService
{
    Task UpdateMany<T>(string organizationId, string userId, string filterProperty, string filterValue, string property, object? value)
        where T : IDataModel;
}

public class UpdateManyPropertyService : IUpdateManyPropertyService
{
    private readonly ILogger<UpdateManyPropertyService> _logger;
    private readonly IOrganizationContext _organizationContext;

    public UpdateManyPropertyService(ILogger<UpdateManyPropertyService> logger, IOrganizationContext organizationContext)
    {
        _logger = logger;
        _organizationContext = organizationContext;
    }

    public async Task UpdateMany<T>(string organizationId, string userId, string filterProperty, string filterValue, string property, object? value)
        where T : IDataModel
    {
        if (string.IsNullOrEmpty(filterProperty))
        {
            _logger.LogWarning("Cannot update the property for type {itemType} and empty id", typeof(T).Name);
            return;
        }

        _organizationContext.SelectOrganization(organizationId);
        var collection = _organizationContext.GetCollection<T>();
        await _logger.LogPropertyManyUpdate<T>(userId, property, filterProperty, async () =>
        {
            var result = await collection.UpdateManyAsync(Builders<T>.Filter.Eq(filterProperty, filterValue), Builders<T>.Update.Set(property, value));

            return result.ModifiedCount;
        });
    }
}