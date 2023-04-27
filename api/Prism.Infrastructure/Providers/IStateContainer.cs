namespace Prism.Infrastructure.Providers;

public interface IStateContainer<T>
{
    Task DeleteAsync(string id);
    Task<IEnumerable<TField>> Distinct<TField>(string field, params Filter[] filters);
    Task<IEnumerable<T>> FetchAsync(params Filter[] filters);
    Task<IEnumerable<T>> ListAsync();
    Task<T?> ReadAsync(string id);
    Task<IEnumerable<T>> SearchAsync(params Filter[] request);
    Task WriteAsync(string id, T value);
}