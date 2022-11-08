// -----------------------------------------------------------------------
//  <copyright file = "SearchMeetings.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Queries;

public record SearchMeetings(DateTime StartDate, DateTime EndDate) : IRequest<List<Meeting>>;

public class SearchMeetingsHandler : IRequestHandler<SearchMeetings, List<Meeting>>
{
    private readonly IOrganizationContext _organizationContext;

    public SearchMeetingsHandler(IOrganizationContext organizationContext)
    {
        _organizationContext = organizationContext;
    }

    public async Task<List<Meeting>> Handle(SearchMeetings request, CancellationToken cancellationToken)
    {
        var filters = Builders<Meeting>.Filter.And(new List<FilterDefinition<Meeting>>
        {
            Builders<Meeting>.Filter.Gte(x => x.StartDate, request.StartDate),
            Builders<Meeting>.Filter.Lte(x => x.StartDate, request.EndDate)
        });

        var collection = _organizationContext.GetCollection<Meeting>();
        var results = await collection.FindAsync(Builders<Meeting>.Filter.And(filters), cancellationToken: cancellationToken);

        return await results.ToListAsync(cancellationToken: cancellationToken);
    }
}