namespace Prism.Infrastructure.Providers;

public interface IStateContainer<T>
{
    Task<T?> ReadAsync(string id);
    
    Task WriteAsync(string id, T value);
    Task<IEnumerable<T>> FetchAsync(params Filter[] filters);
}