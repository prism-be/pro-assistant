// -----------------------------------------------------------------------
//  <copyright file = "FindMany.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Queries;

public record FindMany<T> : IRequest<List<T>>;

public class FindManyHandler<T> : IRequestHandler<FindMany<T>, List<T>>
{
    private readonly IOrganizationContext _organizationContext;

    public FindManyHandler(IOrganizationContext organizationContext)
    {
        _organizationContext = organizationContext;
    }

    public async Task<List<T>> Handle(FindMany<T> request, CancellationToken cancellationToken)
    {
        var collection = _organizationContext.GetCollection<T>();
        var results = await collection.FindAsync<T>(Builders<T>.Filter.Empty, cancellationToken: cancellationToken);
        return results.ToList(cancellationToken: cancellationToken);
    }
}