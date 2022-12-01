// -----------------------------------------------------------------------
//  <copyright file = "MigrationController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Business.Storage.Migrations;

namespace Prism.ProAssistant.Api.Controllers;

[Authorize]
public class MigrationController : Controller
{
    private readonly ILogger<MigrationController> _logger;
    private readonly IServiceProvider _serviceProvider;

    public MigrationController(ILogger<MigrationController> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    [HttpPost]
    [Route("api/admin/migrate/{migrationName}")]
    public async Task ExecuteMigration([FromRoute]string migrationName)
    {
        switch (migrationName)
        {
            case nameof(MigrateDocumentConfiguration):
                await _serviceProvider.GetService<IMigrateDocumentConfiguration>()?.MigrateAsync()!;
                return;
        }

        _logger.LogWarning("Migration {migrationName} not found", migrationName);
    }
}