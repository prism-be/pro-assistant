// -----------------------------------------------------------------------
//  <copyright file = "CheckCache.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prism.ProAssistant.Business.Security;

namespace Prism.ProAssistant.Api.HealthChecks;

public class CheckCache : IHealthCheck
{
    private readonly IDistributedCache _cache;

    public CheckCache(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        try
        {
            var id = Identifier.GenerateString();
            var data = Encoding.UTF8.GetBytes(id);

            await _cache.SetAsync(id, data, cancellationToken);
            await _cache.RemoveAsync(id, cancellationToken);

            return HealthCheckResult.Healthy("The cache is healthy.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("The cache is unhealthy.", ex);
        }
    }
}