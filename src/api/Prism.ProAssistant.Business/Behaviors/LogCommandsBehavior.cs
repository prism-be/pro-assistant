// -----------------------------------------------------------------------
//  <copyright file = "ValidationBehavior.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Prism.ProAssistant.Business.Behaviors;

public sealed class LogCommandsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    private readonly ILogger<LogCommandsBehavior<TRequest, TResponse>> _logger;

    public LogCommandsBehavior(ILogger<LogCommandsBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var organizationId = Guid.Empty.ToString();
        var organizationIdProperty = typeof(TRequest).GetProperty("OrganizationId");

        if (organizationIdProperty != null)
        {
            organizationId = organizationIdProperty.GetValue(request)?.ToString();
        }
        
        var id = Guid.Empty.ToString();
        var idProperty = typeof(TRequest).GetProperty("Id");

        if (idProperty != null)
        {
            id = idProperty.GetValue(request)?.ToString();
        }

        var userId = Guid.Empty.ToString();
        var userIdProperty = typeof(TRequest).GetProperty("UserId");

        if (userIdProperty != null)
        {
            userId = userIdProperty.GetValue(request)?.ToString();
        }
        
        _logger.LogDebug("Processing request '{command}' (organization: {organizationId} - id: {id} - userId: {userId})", typeof(TRequest).FullName, organizationId, id, userId);
        
        
        var sw = new Stopwatch();
        sw.Start();
        var reponse = await next();
        sw.Stop();
        
        _logger.LogDebug("Processed request '{command}' (organization: {organizationId} - id: {id} - userId: {userId}) - {elapsed}", typeof(TRequest).FullName, organizationId, id, userId, sw.Elapsed);
        
        return reponse;
    }
}