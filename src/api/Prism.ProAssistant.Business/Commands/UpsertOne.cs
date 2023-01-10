// -----------------------------------------------------------------------
//  <copyright file = "UpsertOne.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Extensions;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Commands;

public record UpsertOne<T>(T Item) : IRequest<UpsertResult>
    where T : IDataModel;

public record UpsertResult(string Id, string Organization);

public class UpsertOneHandler<T> : IRequestHandler<UpsertOne<T>, UpsertResult>
    where T : IDataModel
{
    private readonly ILogger<UpsertOneHandler<T>> _logger;

    private readonly IOrganizationContext _organizationContext;
    private readonly IUserContextAccessor _userContextAccessor;

    public UpsertOneHandler(ILogger<UpsertOneHandler<T>> logger, IOrganizationContext organizationContext, IUserContextAccessor userContextAccessor)
    {
        _organizationContext = organizationContext;
        _logger = logger;
        _userContextAccessor = userContextAccessor;
    }

    public async Task<UpsertResult> Handle(UpsertOne<T> request, CancellationToken cancellationToken)
    {
        var collection = _organizationContext.GetCollection<T>();

        if (string.IsNullOrWhiteSpace(request.Item.Id))
        {
            return await _logger.LogDataInsert(_userContextAccessor, request.Item, async () =>
            {
                await collection.InsertOneAsync(request.Item, cancellationToken: cancellationToken);
                return new UpsertResult(request.Item.Id, _organizationContext.OrganizationId);
            });
        }

        return await _logger.LogDataUpdate(_userContextAccessor, request.Item, async () =>
        {
            var updated = await collection.FindOneAndReplaceAsync(Builders<T>.Filter.Eq("Id", request.Item.Id), request.Item, new FindOneAndReplaceOptions<T>
            {
                IsUpsert = true
            }, cancellationToken);

            return new UpsertResult(updated.Id, _organizationContext.OrganizationId);
        });
    }
}