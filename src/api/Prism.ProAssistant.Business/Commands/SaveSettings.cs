// -----------------------------------------------------------------------
//  <copyright file = "SaveSettings.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Prism.ProAssistant.Business.Extensions;
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
        var collection = _organizationContext.GetCollection<Setting>();

        foreach (var setting in request.Settings)
        {
            var query = await collection.FindAsync(Builders<Setting>.Filter.Eq("Id", setting.Id), cancellationToken: cancellationToken);
            var existingSettings = await query.FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (existingSettings == null)
            {
                await _logger.LogDataInsert(_userContextAccessor, setting, async () =>
                {
                    await collection.InsertOneAsync(setting, cancellationToken: cancellationToken);
                    return new UpsertResult(setting.Id, _userContextAccessor.UserId);
                });

                return Unit.Value;
            }

            await _logger.LogDataUpdate(_userContextAccessor, setting, async () =>
            {
                await collection.FindOneAndReplaceAsync(Builders<Setting>.Filter.Eq("Id", setting.Id), setting, new FindOneAndReplaceOptions<Setting>
                {
                    IsUpsert = true
                }, cancellationToken);

                return new UpsertResult(setting.Id, _userContextAccessor.UserId);
            });
        }

        return Unit.Value;
    }
}