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

public record SearchMeetings(DateTime StartDate, DateTime EndDate, string? PatientId) : IRequest<List<Meeting>>;

public class SearchMeetingsHandler : IRequestHandler<SearchMeetings, List<Meeting>>
{
    private readonly IOrganizationContext _organizationContext;

    public SearchMeetingsHandler(IOrganizationContext organizationContext)
    {
        _organizationContext = organizationContext;
    }

    public async Task<List<Meeting>> Handle(SearchMeetings request, CancellationToken cancellationToken)
    {
        var filters = new List<FilterDefinition<Meeting>>();
        
        if (request.StartDate != DateTime.MinValue)
        {
            filters.Add(Builders<Meeting>.Filter.Gte(x => x.StartDate, request.StartDate));
        }
        
        if (request.EndDate != DateTime.MinValue)
        {
            filters.Add(Builders<Meeting>.Filter.Lte(x => x.StartDate, request.EndDate));
        }

        if (!string.IsNullOrWhiteSpace(request.PatientId))
        {
            filters.Add(Builders<Meeting>.Filter.Eq(x => x.PatientId, request.PatientId));
        }

        var collection = _organizationContext.GetCollection<Meeting>();
        var results = await collection.FindAsync(Builders<Meeting>.Filter.And(filters), cancellationToken: cancellationToken);

        var items = await results.ToListAsync(cancellationToken: cancellationToken);

        return items.OrderBy(x => x.StartDate).ToList();
    }
}