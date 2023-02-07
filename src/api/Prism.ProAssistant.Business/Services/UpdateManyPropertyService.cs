// -----------------------------------------------------------------------
//  <copyright file = "UpdateManyPropertyService.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Extensions;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Services;

public interface IUpdateManyPropertyService
{
    Task UpdateMany<T>(string filterProperty, string filterValue, string property, object? value)
        where T : IDataModel;
}

public class UpdateManyPropertyService : IUpdateManyPropertyService
{
    private readonly ILogger<UpdateManyPropertyService> _logger;
    private readonly IOrganizationContext _organizationContext;
    private readonly User _user;

    public UpdateManyPropertyService(ILogger<UpdateManyPropertyService> logger, IOrganizationContext organizationContext, User user)
    {
        _logger = logger;
        _organizationContext = organizationContext;
        _user = user;
    }

    public async Task UpdateMany<T>(string filterProperty, string filterValue, string property, object? value)
        where T : IDataModel
    {
        if (string.IsNullOrEmpty(filterProperty))
        {
            _logger.LogWarning("Cannot update the property for type {itemType} and empty id", typeof(T).Name);
            return;
        }

        var collection = _organizationContext.GetCollection<T>();
        await _logger.LogPropertyManyUpdate<T>(_user, property, filterProperty, async () =>
        {
            var result = await collection.UpdateManyAsync(Builders<T>.Filter.Eq(filterProperty, filterValue), Builders<T>.Update.Set(property, value));

            return result.ModifiedCount;
        });
    }
}