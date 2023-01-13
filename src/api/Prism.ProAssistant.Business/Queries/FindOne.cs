// -----------------------------------------------------------------------
//  <copyright file = "FindOne.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Queries;

public record FindOne<T>(string Id) : IRequest<T?>;

public class FindOneHandler<T> : IRequestHandler<FindOne<T>, T?>
{
    private readonly IOrganizationContext _organizationContext;

    public FindOneHandler(IOrganizationContext organizationContext)
    {
        _organizationContext = organizationContext;
    }

    public async Task<T?> Handle(FindOne<T> request, CancellationToken cancellationToken)
    {
        var collection = _organizationContext.GetCollection<T>();
        var query = await collection.FindAsync<T>(Builders<T>.Filter.Eq("Id", request.Id), cancellationToken: cancellationToken);
        return await query.SingleOrDefaultAsync(cancellationToken);
    }
}