namespace Prism.Infrastructure.Providers;

public interface IGlobalStateProvider
{
    Task<IStateContainer<T>> GetGlobalContainerAsync<T>();
}