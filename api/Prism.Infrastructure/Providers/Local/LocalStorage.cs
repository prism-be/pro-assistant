using System.Text.Json;
using Prism.Core;
using Prism.Core.Exceptions;

namespace Prism.Infrastructure.Providers.Local;

public class LocalStorage : IDataStorage
{

    public Task<Stream> CreateFileStreamAsync(string organization, string container, string fileName, string id)
    {
        var directory = EnvironmentConfiguration.GetMandatoryConfiguration("STORAGE_DIRECTORY");
        Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(directory, container))!);
        var path = Path.Combine(directory, container, id);

        var metaData = new FileMetaData(fileName);
        var metaDataPath = Path.Combine(directory, organization, container, $"{id}.meta");
        File.WriteAllText(metaDataPath, JsonSerializer.Serialize(metaData));

        return Task.FromResult<Stream>(File.Open(path, FileMode.Create));
    }

    public Task<bool> ExistsAsync(string organization, string container, string id)
    {
        var directory = EnvironmentConfiguration.GetMandatoryConfiguration("STORAGE_DIRECTORY");
        var path = Path.Combine(directory, organization, container, id);
        return Task.FromResult(File.Exists(path));
    }

    public Task<string> GetFileNameAsync(string organization, string container, string id)
    {
        var directory = EnvironmentConfiguration.GetMandatoryConfiguration("STORAGE_DIRECTORY");
        var metaDataPath = Path.Combine(directory, organization, container, $"{id}.meta");
        var metaData = JsonSerializer.Deserialize<FileMetaData>(File.ReadAllText(metaDataPath));

        if (metaData is null)
        {
            throw new NotFoundException("File meta data not found");
        }

        return Task.FromResult(metaData.FileName);
    }

    public Task<Stream> OpenFileStreamAsync(string organization, string container, string id)
    {
        var directory = EnvironmentConfiguration.GetMandatoryConfiguration("STORAGE_DIRECTORY");
        var path = Path.Combine(directory, organization, container, id);
        return Task.FromResult<Stream>(File.Open(path, FileMode.Open));
    }

    private record FileMetaData(string FileName);
}