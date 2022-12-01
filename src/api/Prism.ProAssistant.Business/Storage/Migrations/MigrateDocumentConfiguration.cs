// -----------------------------------------------------------------------
//  <copyright file = "MigrateDocumentConfiguration.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Models;

namespace Prism.ProAssistant.Business.Storage.Migrations;

public class MigrateDocumentConfiguration : IMigrateDocumentConfiguration
{
    private readonly IOrganizationContext _organizationContext;
    private readonly ILogger<MigrateDocumentConfiguration> _logger;
    
    public MigrateDocumentConfiguration(IOrganizationContext organizationContext, ILogger<MigrateDocumentConfiguration> logger)
    {
        _organizationContext = organizationContext;
        _logger = logger;
    }

    public async Task MigrateAsync()
    {
        if (await _organizationContext.Database.GetCollection<DocumentConfiguration>("documents-configuration").CountDocumentsAsync(FilterDefinition<DocumentConfiguration>.Empty) > 0)
        {
            _logger.LogWarning("Document configuration already migrated");
            return;
        }

        _logger.LogInformation("Migrating document configuration");
        await _organizationContext.Database.RenameCollectionAsync("documents", "documents-configuration");
    }
}