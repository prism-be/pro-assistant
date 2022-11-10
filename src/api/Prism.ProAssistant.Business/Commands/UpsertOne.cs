// -----------------------------------------------------------------------
//  <copyright file = "UpsertOne.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Commands;

public record UpsertOne<T>(T Item) : IRequest<UpsertResult>
    where T : IDataModel;

public record UpsertResult(string Id);

public class UpsertOneHandler<T> : IRequestHandler<UpsertOne<T>, UpsertResult>
    where T : IDataModel
{
    private readonly ILogger<UpsertOneHandler<T>> _logger;

    private readonly IOrganizationContext _organizationContext;
    private readonly IUserContextAccessor _userContextAccessor;

    public UpsertOneHandler(ILogger<UpsertOneHandler<T>> logger,IOrganizationContext organizationContext,  IUserContextAccessor userContextAccessor)
    {
        _organizationContext = organizationContext;
        _logger = logger;
        _userContextAccessor = userContextAccessor;
    }

    public async Task<UpsertResult> Handle(UpsertOne<T> request, CancellationToken cancellationToken)
    {
        var history = _organizationContext.GetCollection<History>();
        var collection = _organizationContext.GetCollection<T>();

        if (string.IsNullOrWhiteSpace(request.Item.Id))
        {
            _logger.LogInformation("Inserting an new item of type {itemType} by user {userId}", typeof(T).FullName, _userContextAccessor.UserId);
            
            await history.InsertOneAsync(new History(_userContextAccessor.UserId, request.Item), cancellationToken: cancellationToken);
            await collection.InsertOneAsync(request.Item, cancellationToken: cancellationToken);
            
            _logger.LogInformation("Inserted an new item of type {itemType} with id {itemId} by user {userId}", typeof(T).FullName, request.Item.Id, _userContextAccessor.UserId);
            
            return new UpsertResult(request.Item.Id);
        }
        
        _logger.LogInformation("Updating an new item of type {itemType} with id {itemId} by user {userId}", typeof(T).FullName, request.Item.Id, _userContextAccessor.UserId);

        await history.InsertOneAsync(new History(_userContextAccessor.UserId, request.Item), cancellationToken: cancellationToken);
        var updated = await collection.FindOneAndReplaceAsync(Builders<T>.Filter.Eq("Id", request.Item.Id), request.Item, new FindOneAndReplaceOptions<T>
        {
            IsUpsert = true
        }, cancellationToken);
        
        _logger.LogInformation("Updated an new item of type {itemType} with id {itemId} by user {userId}", typeof(T).FullName, request.Item.Id, _userContextAccessor.UserId);

        return new UpsertResult(updated.Id);
    }
}