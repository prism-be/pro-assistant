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

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var organisationId = Guid.Empty.ToString();
        var organisationIdProperty = typeof(TRequest).GetProperty("OrganisationId");

        if (organisationIdProperty != null)
        {
            organisationId = organisationIdProperty.GetValue(request)?.ToString();
        }
        
        var id = Guid.Empty.ToString();
        var idProperty = typeof(TRequest).GetProperty("Id");

        if (idProperty != null)
        {
            id = idProperty.GetValue(request)?.ToString();
        }

        _logger.LogInformation("Processing request '{command}' (organisation: {organisationId} - id: {id})", typeof(TRequest).FullName, organisationId, id);
        
        
        var sw = new Stopwatch();
        sw.Start();
        var reponse = await next();
        sw.Stop();
        
        _logger.LogInformation("Processed request '{command}' (organisation: {organisationId} - id: {id}) - {elapsed}", typeof(TRequest).FullName, organisationId, id, sw.Elapsed);
        
        return reponse;
    }
}