// -----------------------------------------------------------------------
//  <copyright file = "PatientQuery.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.RegularExpressions;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Api.Graph.Patients;

[Authorize]
[ExtendObjectType("Query")]
public class PatientQuery
{
    [UseFirstOrDefault]
    public IExecutable<Patient> GetPatientById(string id, [Service] IOrganizationContext organizationContext, [Service] ILogger<PatientQuery> logger,
        [Service] IUserContextAccessor userContextAccessor)
    {
        logger.LogInformation("GDRP : {userId} is accessing patient data {patientId}", userContextAccessor.UserId, id);
        return organizationContext.Patients.Find(x => x.Id == id).AsExecutable();
    }

    [UsePaging]
    [UseProjection]
    [UseSorting]
    [UseFiltering]
    public IExecutable<Patient> GetPatients([Service] IOrganizationContext organizationContext, [Service] ILogger<PatientQuery> logger, [Service] IUserContextAccessor userContextAccessor)
    {
        logger.LogInformation("GDRP : {userId} is searching patients and read summary", userContextAccessor.UserId);
        return organizationContext.Patients.AsExecutable();
    }

    [UseSorting]
    public IExecutable<Patient> SearchPatients(string lastName, string firstName, string phoneNumber, string birthDate, [Service] IOrganizationContext organizationContext,
        [Service] ILogger<PatientQuery> logger, [Service] IUserContextAccessor userContextAccessor)
    {
        logger.LogInformation("GDRP : {userId} is searching patients (query : {lastName}, {firstName}, {phoneNumber}, {birthDate}) and read summary",
            userContextAccessor.UserId,
            lastName,
            firstName,
            phoneNumber,
            birthDate);

        var filters = new List<FilterDefinition<Patient>>();

        if (!string.IsNullOrWhiteSpace(lastName))
        {
            filters.Add(Builders<Patient>.Filter.Regex(x => x.LastName, BsonRegularExpression.Create(new Regex($"^{lastName}", RegexOptions.IgnoreCase))));
        }

        if (!string.IsNullOrWhiteSpace(firstName))
        {
            filters.Add(Builders<Patient>.Filter.Regex(x => x.FirstName, BsonRegularExpression.Create(new Regex($"^{firstName}", RegexOptions.IgnoreCase))));
        }

        if (!string.IsNullOrWhiteSpace(phoneNumber))
        {
            filters.Add(Builders<Patient>.Filter.Regex(x => x.PhoneNumber, BsonRegularExpression.Create(new Regex($"^{phoneNumber}", RegexOptions.IgnoreCase))));
        }

        if (!string.IsNullOrWhiteSpace(birthDate))
        {
            filters.Add(Builders<Patient>.Filter.Regex(x => x.BirthDate, BsonRegularExpression.Create(new Regex($"^{birthDate}", RegexOptions.IgnoreCase))));
        }

        var filter = filters.Count == 0
            ? Builders<Patient>.Filter.Empty
            : Builders<Patient>.Filter.And(filters);

        return organizationContext.Patients
            .Find(filter)
            .AsExecutable();
    }
}