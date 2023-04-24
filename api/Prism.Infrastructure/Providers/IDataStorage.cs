namespace Prism.Infrastructure.Providers;

public interface IDataStorage
{
    Task<Stream> CreateFileStreamAsync(string organization, string container, string fileName, string id);

    Task<bool> ExistsAsync(string organization, string container, string id);

    Task<string> GetFileNameAsync(string organization, string container, string id);

    Task<Stream> OpenFileStreamAsync(string organization, string container, string id);
}