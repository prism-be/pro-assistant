// -----------------------------------------------------------------------
//  <copyright file = "UpsertManyService.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.ProAssistant.Business.Models;

namespace Prism.ProAssistant.Business.Services;

public interface IUpsertManyService
{
    Task<List<UpsertResult>> Upsert<T>(List<T> items)
        where T : IDataModel;
}

public class UpsertManyService : IUpsertManyService
{
    private readonly IUpsertOneService _upsertOneService;

    public UpsertManyService(IUpsertOneService upsertOneService)
    {
        _upsertOneService = upsertOneService;
    }

    public async Task<List<UpsertResult>> Upsert<T>(List<T> items)
        where T : IDataModel
    {
        var results = new List<UpsertResult>();

        foreach (var item in items)
        {
            results.Add(await _upsertOneService.Upsert(item));
        }

        return results;
    }
}