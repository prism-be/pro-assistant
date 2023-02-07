// -----------------------------------------------------------------------
//  <copyright file = "CrudService.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.ProAssistant.Business.Models;

namespace Prism.ProAssistant.Business.Services;

public interface ICrudService
{
    Task<List<T>> FindMany<T>()
        where T : IDataModel;

    Task<T?> FindOne<T>(string id)
        where T : IDataModel;

    Task RemoveOne<T>(string id)
        where T : IDataModel;

    Task UpdateManyProperty<T>(string filterProperty, string filterValue, string property, object? value)
        where T : IDataModel;

    Task UpdateProperty<T>(string id, string property, object value)
        where T : IDataModel;

    Task UpsertMany<T>(List<T> items)
        where T : IDataModel;

    Task UpsertOne<T>(T item)
        where T : IDataModel;
}

public class CrudService : ICrudService
{
    private readonly IFindManyService _findManyService;
    private readonly IFindOneService _findOneService;
    private readonly IRemoveOneService _removeOneService;
    private readonly IUpdateManyPropertyService _updateManyPropertyService;
    private readonly IUpdatePropertyService _updatePropertyService;
    private readonly IUpsertManyService _upsertManyService;
    private readonly IUpsertOneService _upsertOneService;

    public CrudService(IFindManyService findManyService, IFindOneService findOneService, IRemoveOneService removeOneService, IUpdateManyPropertyService updateManyPropertyService,
        IUpdatePropertyService updatePropertyService, IUpsertManyService upsertManyService, IUpsertOneService upsertOneService)
    {
        _findManyService = findManyService;
        _findOneService = findOneService;
        _removeOneService = removeOneService;
        _updateManyPropertyService = updateManyPropertyService;
        _updatePropertyService = updatePropertyService;
        _upsertManyService = upsertManyService;
        _upsertOneService = upsertOneService;
    }

    public async Task<List<T>> FindMany<T>()
        where T : IDataModel
    {
        return await _findManyService.Find<T>();
    }

    public async Task<T?> FindOne<T>(string id)
        where T : IDataModel
    {
        return await _findOneService.Find<T>(id);
    }

    public async Task RemoveOne<T>(string id)
        where T : IDataModel
    {
        await _removeOneService.Remove<T>(id);
    }

    public async Task UpdateManyProperty<T>(string filterProperty, string filterValue, string property, object? value)
        where T : IDataModel
    {
        await _updateManyPropertyService.Update<T>(filterProperty, filterValue, property, value);
    }

    public async Task UpdateProperty<T>(string id, string property, object value)
        where T : IDataModel
    {
        await _updatePropertyService.Update<T>(id, property, value);
    }

    public async Task UpsertMany<T>(List<T> items)
        where T : IDataModel
    {
        await _upsertManyService.Upsert(items);
    }

    public async Task UpsertOne<T>(T item)
        where T : IDataModel
    {
        await _upsertOneService.Upsert(item);
    }
}