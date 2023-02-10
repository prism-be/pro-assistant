// -----------------------------------------------------------------------
//  <copyright file = "SearchContacts.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Services;

public interface ISearchContactsService
{
    Task<List<Contact>> Search(string lastName, string firstName, string phoneNumber, string birthDate);
}

public class SearchContactsService : ISearchContactsService
{
    private readonly ILogger<SearchContactsService> _logger;
    private readonly IOrganizationContext _organizationContext;
    private readonly User _user;

    public SearchContactsService(IOrganizationContext organizationContext, ILogger<SearchContactsService> logger, User user)
    {
        _organizationContext = organizationContext;
        _logger = logger;
        _user = user;
    }

    public async Task<List<Contact>> Search(string lastName, string firstName, string phoneNumber, string birthDate)
    {
        _logger.LogInformation("GDPR : {userId} is searching contacts (query : {lastName}, {firstName}, {phoneNumber}, {birthDate}) and read summary",
            _user.Id,
            lastName,
            firstName,
            phoneNumber,
            birthDate);

        var filters = new List<FilterDefinition<Contact>>();

        if (!string.IsNullOrWhiteSpace(lastName))
        {
            filters.Add(Builders<Contact>.Filter.Regex(x => x.LastName,
                BsonRegularExpression.Create(new Regex($"^{lastName}", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100)))));
        }

        if (!string.IsNullOrWhiteSpace(firstName))
        {
            filters.Add(Builders<Contact>.Filter.Regex(x => x.FirstName,
                BsonRegularExpression.Create(new Regex($"^{firstName}", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100)))));
        }

        if (!string.IsNullOrWhiteSpace(phoneNumber))
        {
            filters.Add(Builders<Contact>.Filter.Regex(x => x.PhoneNumber,
                BsonRegularExpression.Create(new Regex($"^{phoneNumber}", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100)))));
        }

        if (!string.IsNullOrWhiteSpace(birthDate))
        {
            filters.Add(Builders<Contact>.Filter.Regex(x => x.BirthDate,
                BsonRegularExpression.Create(new Regex($"^{birthDate}", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100)))));
        }

        var filter = filters.Count == 0
            ? Builders<Contact>.Filter.Empty
            : Builders<Contact>.Filter.And(filters);

        var collection = _organizationContext.GetCollection<Contact>();
        var results = await collection.FindAsync(filter);

        return await results.ToListAsync();
    }
}