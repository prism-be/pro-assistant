// -----------------------------------------------------------------------
//  <copyright file = "SearchAppointmentsService.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MongoDB.Driver;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Queries;

public interface ISearchAppointmentsService
{
    Task<List<Appointment>> Search(DateTime startDate, DateTime endDate, string? contactId);
}

public class SearchAppointmentsService : ISearchAppointmentsService
{
    private readonly IOrganizationContext _organizationContext;

    public SearchAppointmentsService(IOrganizationContext organizationContext)
    {
        _organizationContext = organizationContext;
    }

    public async Task<List<Appointment>> Search(DateTime startDate, DateTime endDate, string? contactId)
    {
        var filters = new List<FilterDefinition<Appointment>>();

        if (startDate != DateTime.MinValue)
        {
            filters.Add(Builders<Appointment>.Filter.Gte(x => x.StartDate, startDate));
        }

        if (endDate != DateTime.MinValue)
        {
            filters.Add(Builders<Appointment>.Filter.Lte(x => x.StartDate, endDate));
        }

        if (!string.IsNullOrWhiteSpace(contactId))
        {
            filters.Add(Builders<Appointment>.Filter.Eq(x => x.ContactId, contactId));
        }

        filters.Add(Builders<Appointment>.Filter.Ne(x => x.State, (int)AppointmentState.Canceled));

        var collection = _organizationContext.GetCollection<Appointment>();
        var results = await collection.FindAsync(Builders<Appointment>.Filter.And(filters));

        var items = await results.ToListAsync();

        return items.OrderBy(x => x.StartDate).ToList();
    }
}