// -----------------------------------------------------------------------
//  <copyright file = "SaveSettings.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Business.Commands;

public record SaveSettings(List<Setting> Settings) : IRequest;

public class SaveSettingsHandler : IRequestHandler<SaveSettings>
{
    private readonly ILogger<SaveSettingsHandler> _logger;

    private readonly IOrganizationContext _organizationContext;
    private readonly IUserContextAccessor _userContextAccessor;

    public SaveSettingsHandler(ILogger<SaveSettingsHandler> logger, IOrganizationContext organizationContext, IUserContextAccessor userContextAccessor)
    {
        _organizationContext = organizationContext;
        _logger = logger;
        _userContextAccessor = userContextAccessor;
    }

    public async Task<Unit> Handle(SaveSettings request, CancellationToken cancellationToken)
    {
        var history = _organizationContext.GetCollection<History>();
        var collection = _organizationContext.GetCollection<Setting>();

        foreach (var setting in request.Settings)
        {
            var query = await collection.FindAsync(Builders<Setting>.Filter.Eq("Id", setting.Id), cancellationToken: cancellationToken);
            var existingSettings = await query.FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (existingSettings == null)
            {
                _logger.LogInformation("Inserting an new setting with id {SettingId}", setting.Id);

                await collection.InsertOneAsync(setting, cancellationToken: cancellationToken);
                await history.InsertOneAsync(new History(_userContextAccessor.UserId, setting), cancellationToken: cancellationToken);

                _logger.LogInformation("Inserted an new setting with id {SettingId}", setting.Id);

                return Unit.Value;
            }

            _logger.LogInformation("Updating an existing setting with id {SettingId}", setting.Id);

            await history.InsertOneAsync(new History(_userContextAccessor.UserId, setting), cancellationToken: cancellationToken);
            await collection.FindOneAndReplaceAsync(Builders<Setting>.Filter.Eq("Id", setting.Id), setting, new FindOneAndReplaceOptions<Setting>
            {
                IsUpsert = true
            }, cancellationToken);

            _logger.LogInformation("Updated an existing setting with id {SettingId}", setting.Id);
        }

        return Unit.Value;
    }
}