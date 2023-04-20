using Microsoft.Extensions.Logging;
using Prism.Core.Exceptions;
using Prism.Infrastructure.Authentication;
using Prism.Infrastructure.Providers;

namespace Prism.ProAssistant.Storage;

public interface IQueryService
{
    Task<IEnumerable<T>> ListAsync<T>();
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

    public async Task<IEnumerable<T>> ListAsync<T>()
    {
        _logger.LogInformation("ListAsync - {Type} - {UserId}", typeof(T).Name, _userOrganization.Id);

        var container = await _stateProvider.GetContainerAsync<T>();
        return await container.ListAsync();
    }

    public async Task<T> SingleAsync<T>(string id)
    {
        _logger.LogInformation("SingleAsync - {Type}({ItemId}) - {UserId}", typeof(T).Name, id, _userOrganization.Id);

        var container = await _stateProvider.GetContainerAsync<T>();
        var item = await container.ReadAsync(id);
        return item ?? throw new NotFoundException($"Item {id} of type {typeof(T).Name} not found");
    }

    public async Task<T?> SingleOrDefaultAsync<T>(string id)
    {
        _logger.LogInformation("SingleOrDefaultAsync - {Type}({ItemId}) - {UserId}", typeof(T).Name, id, _userOrganization.Id);

        if (id == "000000000000000000000000")
        {
            return default;
        }

        var container = await _stateProvider.GetContainerAsync<T>();
        return await container.ReadAsync(id);
    }

    public async Task<IEnumerable<T>> SearchAsync<T>(params Filter[] request)
    {
        _logger.LogInformation("SearchAsync - {Type} - {UserId}", typeof(T).Name, _userOrganization.Id);

        var container = await _stateProvider.GetContainerAsync<T>();
        return await container.SearchAsync(request);
    }
}