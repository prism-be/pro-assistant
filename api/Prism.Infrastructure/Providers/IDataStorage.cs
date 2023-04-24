namespace Prism.Infrastructure.Providers;

public interface IDataStorage
{
    Task<Stream> CreateFileStreamAsync(string container, string fileName, string id);

    Task<bool> ExistsAsync(string container, string id);

    Task<string> GetFileNameAsync(string container, string id);

    Task<Stream> OpenFileStreamAsync(string container, string id);
}