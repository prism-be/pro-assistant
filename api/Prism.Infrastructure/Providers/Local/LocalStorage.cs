using System.Text.Json;
using Prism.Core;
using Prism.Core.Exceptions;

namespace Prism.Infrastructure.Providers.Local;

public class LocalStorage : IDataStorage
{

    public Task<Stream> CreateFileStreamAsync(string organization, string container, string fileName, string id)
    {
        var path = Path.Combine(GetPath(organization, container), id);
        var metaDataPath = Path.Combine(GetPath(organization, container), $"{id}.meta");

        var metaData = new FileMetaData(fileName);
        File.WriteAllText(metaDataPath, JsonSerializer.Serialize(metaData));

        return Task.FromResult<Stream>(File.Open(path, FileMode.Create));
    }
    
    private static string GetPath(string organization, string container)
    {
        var directory = EnvironmentConfiguration.GetMandatoryConfiguration("STORAGE_DIRECTORY");
        var path = Path.Combine(directory, organization, container);
        Directory.CreateDirectory(path);
        return path;
    }

    public Task<bool> ExistsAsync(string organization, string container, string id)
    {
        var path = Path.Combine(GetPath(organization, container), id);
        return Task.FromResult(File.Exists(path));
    }

    public Task<string> GetFileNameAsync(string organization, string container, string id)
    {
        if (!ExistsAsync(organization, container, id).Result)
        {
            throw new NotFoundException("File not found");
        }
        
        var metaDataPath = Path.Combine(GetPath(organization, container), $"{id}.meta");
        var metaData = JsonSerializer.Deserialize<FileMetaData>(File.ReadAllText(metaDataPath));

        if (metaData is null)
        {
            throw new NotFoundException("File meta data not found");
        }

        return Task.FromResult(metaData.FileName);
    }

    public Task<Stream> OpenFileStreamAsync(string organization, string container, string id)
    {
        var path = Path.Combine(GetPath(organization, container), id);
        return Task.FromResult<Stream>(File.Open(path, FileMode.Open));
    }

    public Task DeleteAsync(string organization, string container, string id)
    {
        var path = Path.Combine(GetPath(organization, container), id);
        File.Delete(path);
        return Task.CompletedTask;
    }

    private record FileMetaData(string FileName);
}