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
    private readonly IUserContextAccessor _userContextAccessor;

    public OrganizationContextEnricher(IUserContextAccessor userContextAccessor)
    {
        _userContextAccessor = userContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var userIdProperty = propertyFactory.CreateProperty("UserId", _userContextAccessor.UserId);
        logEvent.AddPropertyIfAbsent(userIdProperty);

        var organizationIdProperty = propertyFactory.CreateProperty("OrganizationId", _userContextAccessor.OrganizationId);
        logEvent.AddPropertyIfAbsent(organizationIdProperty);
    }
}