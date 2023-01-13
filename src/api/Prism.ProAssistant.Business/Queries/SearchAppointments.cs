// -----------------------------------------------------------------------
//  <copyright file = "SearchAppointments.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Queries;

public record SearchAppointments(DateTime StartDate, DateTime EndDate, string? ContactId) : IRequest<List<Appointment>>;

public class SearchAppointmentsHandler : IRequestHandler<SearchAppointments, List<Appointment>>
{
    private readonly IOrganizationContext _organizationContext;

    public SearchAppointmentsHandler(IOrganizationContext organizationContext)
    {
        _organizationContext = organizationContext;
    }

    public async Task<List<Appointment>> Handle(SearchAppointments request, CancellationToken cancellationToken)
    {
        var filters = new List<FilterDefinition<Appointment>>();

        if (request.StartDate != DateTime.MinValue)
        {
            filters.Add(Builders<Appointment>.Filter.Gte(x => x.StartDate, request.StartDate));
        }

        if (request.EndDate != DateTime.MinValue)
        {
            filters.Add(Builders<Appointment>.Filter.Lte(x => x.StartDate, request.EndDate));
        }

        if (!string.IsNullOrWhiteSpace(request.ContactId))
        {
            filters.Add(Builders<Appointment>.Filter.Eq(x => x.ContactId, request.ContactId));
        }

        var collection = _organizationContext.GetCollection<Appointment>();
        var results = await collection.FindAsync(Builders<Appointment>.Filter.And(filters), cancellationToken: cancellationToken);

        var items = await results.ToListAsync(cancellationToken: cancellationToken);

        return items.OrderBy(x => x.StartDate).ToList();
    }
}