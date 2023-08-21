namespace Prism.ProAssistant.Storage;

using Core.Exceptions;
using Infrastructure.Authentication;
using Infrastructure.Providers;
using Microsoft.Extensions.Logging;

public interface IQueryService
{
    Task<IEnumerable<TField>> DistinctAsync<T, TField>(string field, params Filter[] filters);
    Task<IEnumerable<T>> ListAsync<T>();
    Task<TField?> MaxAsync<T, TField>(Func<T, TField> selector, params Filter[] filters);
    Task<IEnumerable<T>> SearchAsync<T>(params Filter[] request);
    Task<T> SingleAsync<T>(string id);
    Task<T?> SingleOrDefaultAsync<T>(string id);
}

public class QueryService : IQueryService
{
    private readonly ILogger<QueryService> _logger;
    private readonly IStateProvider _stateProvider;
    private readonly UserOrganization _userOrganization;

    public QueryService(ILogger<QueryService> logger, UserOrganization userOrganization, IStateProvider stateProvider)
    {
        _logger = logger;
        _userOrganization = userOrganization;
        _stateProvider = stateProvider;
    }

    public async Task<IEnumerable<TField>> DistinctAsync<T, TField>(string field, params Filter[] filters)
    {
        _logger.LogDebug("DistinctAsync - {Type} - {Field} - {UserId}", typeof(T).Name, field, _userOrganization.Id);

        var container = await _stateProvider.GetContainerAsync<T>();
        return await container.Distinct<TField>(field, filters);
    }

    public async Task<IEnumerable<T>> ListAsync<T>()
    {
        _logger.LogDebug("ListAsync - {Type} - {UserId}", typeof(T).Name, _userOrganization.Id);

        var container = await _stateProvider.GetContainerAsync<T>();
        return await container.ListAsync();
    }

    public async Task<T> SingleAsync<T>(string id)
    {
        _logger.LogDebug("SingleAsync - {Type}({ItemId}) - {UserId}", typeof(T).Name, id, _userOrganization.Id);

        var container = await _stateProvider.GetContainerAsync<T>();
        var item = await container.ReadAsync(id);
        return item ?? throw new NotFoundException($"Item {id} of type {typeof(T).Name} not found");
    }

    public async Task<T?> SingleOrDefaultAsync<T>(string id)
    {
        _logger.LogDebug("SingleOrDefaultAsync - {Type}({ItemId}) - {UserId}", typeof(T).Name, id, _userOrganization.Id);

        if (id == "000000000000000000000000")
        {
            return default;
        }

        var container = await _stateProvider.GetContainerAsync<T>();
        return await container.ReadAsync(id);
    }

    public async Task<TField?> MaxAsync<T, TField>(Func<T, TField> selector, params Filter[] filters)
    {
        _logger.LogDebug("Max - {Type} - {UserId}", typeof(T).Name, _userOrganization.Id);

        var container = await _stateProvider.GetContainerAsync<T>();
        var results = await container.SearchAsync(filters);

        return results.Max(selector);
    }

    public async Task<IEnumerable<T>> SearchAsync<T>(params Filter[] request)
    {
        _logger.LogDebug("SearchAsync - {Type} - {UserId}", typeof(T).Name, _userOrganization.Id);

        var container = await _stateProvider.GetContainerAsync<T>();
        return await container.SearchAsync(request);
    }
}