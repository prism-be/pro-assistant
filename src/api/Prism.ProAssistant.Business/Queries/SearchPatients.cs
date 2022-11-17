// -----------------------------------------------------------------------
//  <copyright file = "SearchPatients.cs" company = "Prism">
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

public record SearchPatients(string LastName, string FirstName, string PhoneNumber, string BirthDate) : IRequest<List<Patient>>;

public class SearchPatientsHandler : IRequestHandler<SearchPatients, List<Patient>>
{
    private readonly ILogger<SearchPatientsHandler> _logger;
    private readonly IOrganizationContext _organizationContext;
    private readonly IUserContextAccessor _userContextAccessor;

    public SearchPatientsHandler(IOrganizationContext organizationContext, ILogger<SearchPatientsHandler> logger, IUserContextAccessor userContextAccessor)
    {
        _organizationContext = organizationContext;
        _logger = logger;
        _userContextAccessor = userContextAccessor;
    }

    public async Task<List<Patient>> Handle(SearchPatients request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GDRP : {userId} is searching patients (query : {lastName}, {firstName}, {phoneNumber}, {birthDate}) and read summary",
            _userContextAccessor.UserId,
            request.LastName,
            request.FirstName,
            request.PhoneNumber,
            request.BirthDate);

        var filters = new List<FilterDefinition<Patient>>();

        if (!string.IsNullOrWhiteSpace(request.LastName))
        {
            filters.Add(Builders<Patient>.Filter.Regex(x => x.LastName, BsonRegularExpression.Create(new Regex($"^{request.LastName}", RegexOptions.IgnoreCase))));
        }

        if (!string.IsNullOrWhiteSpace(request.FirstName))
        {
            filters.Add(Builders<Patient>.Filter.Regex(x => x.FirstName, BsonRegularExpression.Create(new Regex($"^{request.FirstName}", RegexOptions.IgnoreCase))));
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            filters.Add(Builders<Patient>.Filter.Regex(x => x.PhoneNumber, BsonRegularExpression.Create(new Regex($"^{request.PhoneNumber}", RegexOptions.IgnoreCase))));
        }

        if (!string.IsNullOrWhiteSpace(request.BirthDate))
        {
            filters.Add(Builders<Patient>.Filter.Regex(x => x.BirthDate, BsonRegularExpression.Create(new Regex($"^{request.BirthDate}", RegexOptions.IgnoreCase))));
        }

        var filter = filters.Count == 0
            ? Builders<Patient>.Filter.Empty
            : Builders<Patient>.Filter.And(filters);

        var collection = _organizationContext.GetCollection<Patient>();
        var results = await collection.FindAsync(filter, cancellationToken: cancellationToken);

        return await results.ToListAsync(cancellationToken: cancellationToken);
    }
}