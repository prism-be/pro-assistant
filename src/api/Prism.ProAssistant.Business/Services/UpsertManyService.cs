// -----------------------------------------------------------------------
//  <copyright file = "UpdateManyService.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.ProAssistant.Business.Models;

namespace Prism.ProAssistant.Business.Services;

public interface IUpsertManyService
{
    Task Upsert<T>(List<T> items)
        where T : IDataModel;
}

public class UpsertManyService : IUpsertManyService
{
    private readonly IUpsertOneService _upsertOneService;

    public UpsertManyService(IUpsertOneService upsertOneService)
    {
        _upsertOneService = upsertOneService;
    }

    public async Task Upsert<T>(List<T> items)
        where T : IDataModel
    {
        foreach (var item in items)
        {
            await _upsertOneService.Upsert(item);
        }
    }
}