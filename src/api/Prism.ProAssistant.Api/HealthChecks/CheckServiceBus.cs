// -----------------------------------------------------------------------
//  <copyright file = "CheckServiceBus.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace Prism.ProAssistant.Api.HealthChecks;

public class CheckServiceBus : IHealthCheck
{
    private readonly IConnection _connection;

    public CheckServiceBus(IConnection connection)
    {
        _connection = connection;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        return Task.FromResult(_connection.IsOpen
            ? HealthCheckResult.Healthy("Service Bus is healthy")
            : HealthCheckResult.Unhealthy("Service Bus is unhealthy"));
    }
}