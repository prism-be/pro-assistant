// -----------------------------------------------------------------------
//  <copyright file = "CheckDatabase.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Prism.ProAssistant.Api.HealthChecks;

public class CheckDatabase : IHealthCheck
{
    private readonly IMongoClient _client;

    public CheckDatabase(IMongoClient client)
    {
        _client = client;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var database = _client.GetDatabase("mydb");
        var command = new BsonDocument("ping", 1);
        var result = database.RunCommand<BsonDocument>(command);
        
        return Task.FromResult(result["ok"] == 1.0 
            ? HealthCheckResult.Healthy("MongoDB is online and available.") 
            : HealthCheckResult.Unhealthy("MongoDB is offline or unavailable."));
    }
}