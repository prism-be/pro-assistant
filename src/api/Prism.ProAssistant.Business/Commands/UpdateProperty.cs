// -----------------------------------------------------------------------
//  <copyright file = "UpsertProperty.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Events;
using Prism.ProAssistant.Business.Extensions;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;
using IPublisher = Prism.ProAssistant.Business.Events.IPublisher;

namespace Prism.ProAssistant.Business.Commands;

// ReSharper disable once UnusedTypeParameter
public record UpdateProperty<T>(string Id, string Property, object Value) : IRequest<UpsertResult>
    where T : IDataModel;

public class UpdatePropertyHandler<T> : IRequestHandler<UpdateProperty<T>, UpsertResult>
    where T : IDataModel
{
    private readonly ILogger<UpsertOneHandler<T>> _logger;

    private readonly IOrganizationContext _organizationContext;
    private readonly IPublisher _publisher;
    private readonly IUserContextAccessor _userContextAccessor;

    public UpdatePropertyHandler(ILogger<UpsertOneHandler<T>> logger, IOrganizationContext organizationContext, IUserContextAccessor userContextAccessor, IPublisher publisher)
    {
        _logger = logger;
        _organizationContext = organizationContext;
        _userContextAccessor = userContextAccessor;
        _publisher = publisher;
    }

    public async Task<UpsertResult> Handle(UpdateProperty<T> request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Id))
        {
            _logger.LogWarning("Cannot update the property for type {itemType} and empty id", typeof(T).Name);
            return new UpsertResult(request.Id, _organizationContext.OrganizationId);
        }

        var collection = _organizationContext.GetCollection<T>();
        return await _logger.LogPropertyUpdate<T>(_userContextAccessor, request.Property, request.Id, async () =>
        {
            var result = await collection.UpdateOneAsync(Builders<T>.Filter.Eq("Id", request.Id), Builders<T>.Update.Set(request.Property, request.Value),
                cancellationToken: cancellationToken);

            if (result.ModifiedCount > 0)
            {
                _publisher.Publish(_userContextAccessor.OrganizationId , _userContextAccessor.UserId, Topics.PropertyUpdated, new PropertyUpdated(typeof(T).Name, request.Id, request.Property, request.Value));
            }

            return new UpsertResult(request.Id, _organizationContext.OrganizationId);
        });
    }
}