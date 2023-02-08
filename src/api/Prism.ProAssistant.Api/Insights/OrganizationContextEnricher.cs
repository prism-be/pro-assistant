// -----------------------------------------------------------------------
//  <copyright file = "OrganizationContextEnricher.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.ProAssistant.Business.Security;
using Serilog.Core;
using Serilog.Events;

namespace Prism.ProAssistant.Api.Insights;

public class OrganizationContextEnricher : ILogEventEnricher
{
    private readonly IUser _user;

    public OrganizationContextEnricher(IUser user)
    {
        _user = user;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var userIdProperty = propertyFactory.CreateProperty("UserId", _user.Id);
        logEvent.AddPropertyIfAbsent(userIdProperty);

        var organizationIdProperty = propertyFactory.CreateProperty("OrganizationId", _user.Organization);
        logEvent.AddPropertyIfAbsent(organizationIdProperty);
    }
}