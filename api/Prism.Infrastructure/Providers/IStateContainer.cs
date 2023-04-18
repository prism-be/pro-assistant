namespace Prism.Infrastructure.Providers;

public interface IStateContainer<T>
{
    Task<IEnumerable<T>> FetchAsync(params Filter[] filters);
    Task<IEnumerable<T>> ListAsync();
    Task<T?> ReadAsync(string id);
    Task<IEnumerable<T>> SearchAsync(IEnumerable<Filter> request);
    Task WriteAsync(string id, T value);
}