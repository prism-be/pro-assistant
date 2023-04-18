namespace Prism.Infrastructure.Providers;

public interface IStateProvider
{
    Task<IStateContainer<T>> GetContainerAsync<T>();
    
    string GenerateUniqueIdentifier();
}