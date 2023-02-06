// -----------------------------------------------------------------------
//  <copyright file = "UpdateManyService.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.ProAssistant.Business.Models;

namespace Prism.ProAssistant.Business.Commands;

public class UpdateManyService
{
    private readonly IUpsertOneService _upsertOneService;

    public UpdateManyService(IUpsertOneService upsertOneService)
    {
        _upsertOneService = upsertOneService;
    }

    public async Task Update<T>(string organisationId, string userId, List<T> items)
        where T : IDataModel
    {
        foreach (var item in items)
        {
            await _upsertOneService.Upsert(organisationId, userId, item);
        }
    }
}