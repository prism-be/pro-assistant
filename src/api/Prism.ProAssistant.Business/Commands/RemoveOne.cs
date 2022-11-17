// -----------------------------------------------------------------------
//  <copyright file = "RemoveOne.cs" company = "Prism">
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

public record RemoveOne<T>(string Id) : IRequest
    where T : IDataModel;

public class RemoveOneHandler<T> : IRequestHandler<RemoveOne<T>>
    where T : IDataModel
{

    private readonly ILogger<RemoveOneHandler<T>> _logger;

    private readonly IOrganizationContext _organizationContext;
    private readonly IUserContextAccessor _userContextAccessor;

    public RemoveOneHandler(ILogger<RemoveOneHandler<T>> logger, IOrganizationContext organizationContext, IUserContextAccessor userContextAccessor)
    {
        _logger = logger;
        _organizationContext = organizationContext;
        _userContextAccessor = userContextAccessor;
    }

    public async Task<Unit> Handle(RemoveOne<T> request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing an new item of type {itemType} with id {itemId} by user {userId}", typeof(T).FullName, request.Id, _userContextAccessor.UserId);

        var collection = _organizationContext.GetCollection<T>();
        await collection.DeleteOneAsync(Builders<T>.Filter.Eq("Id", request.Id), cancellationToken);

        _logger.LogInformation("Removed an new item of type {itemType} with id {itemId} by user {userId}", typeof(T).FullName, request.Id, _userContextAccessor.UserId);

        return Unit.Value;
    }
}