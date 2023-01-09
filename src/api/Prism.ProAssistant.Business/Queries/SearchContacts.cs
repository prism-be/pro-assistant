// -----------------------------------------------------------------------
//  <copyright file = "SearchContacts.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.RegularExpressions;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Queries;

public record SearchContacts(string LastName, string FirstName, string PhoneNumber, string BirthDate) : IRequest<List<Contact>>;

public class SearchContactsHandler : IRequestHandler<SearchContacts, List<Contact>>
{
    private readonly ILogger<SearchContactsHandler> _logger;
    private readonly IOrganizationContext _organizationContext;
    private readonly IUserContextAccessor _userContextAccessor;

    public SearchContactsHandler(IOrganizationContext organizationContext, ILogger<SearchContactsHandler> logger, IUserContextAccessor userContextAccessor)
    {
        _organizationContext = organizationContext;
        _logger = logger;
        _userContextAccessor = userContextAccessor;
    }

    public async Task<List<Contact>> Handle(SearchContacts request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GDPR : {userId} is searching contacts (query : {lastName}, {firstName}, {phoneNumber}, {birthDate}) and read summary",
            _userContextAccessor.UserId,
            request.LastName,
            request.FirstName,
            request.PhoneNumber,
            request.BirthDate);

        var filters = new List<FilterDefinition<Contact>>();

        if (!string.IsNullOrWhiteSpace(request.LastName))
        {
            filters.Add(Builders<Contact>.Filter.Regex(x => x.LastName, BsonRegularExpression.Create(new Regex($"^{request.LastName}", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100)))));
        }

        if (!string.IsNullOrWhiteSpace(request.FirstName))
        {
            filters.Add(Builders<Contact>.Filter.Regex(x => x.FirstName, BsonRegularExpression.Create(new Regex($"^{request.FirstName}", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100)))));
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            filters.Add(Builders<Contact>.Filter.Regex(x => x.PhoneNumber, BsonRegularExpression.Create(new Regex($"^{request.PhoneNumber}", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100)))));
        }

        if (!string.IsNullOrWhiteSpace(request.BirthDate))
        {
            filters.Add(Builders<Contact>.Filter.Regex(x => x.BirthDate, BsonRegularExpression.Create(new Regex($"^{request.BirthDate}", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100)))));
        }

        var filter = filters.Count == 0
            ? Builders<Contact>.Filter.Empty
            : Builders<Contact>.Filter.And(filters);

        var collection = _organizationContext.GetCollection<Contact>();
        var results = await collection.FindAsync(filter, cancellationToken: cancellationToken);

        return await results.ToListAsync(cancellationToken: cancellationToken);
    }
}