using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Prism.Core;

namespace Prism.Infrastructure.Providers.Azure;

public class BlobDataStorage: IDataStorage
{

    public Task<Stream> CreateFileStreamAsync(string container, string fileName, string id)
    {
        var azureBlobClient = GetBlobClient(container);
        var blobClient = azureBlobClient.GetBlobClient(id);
        
        var options = new BlobOpenWriteOptions
        {
            Metadata = new Dictionary<string, string>
            {
                {"FileName", fileName}
            }
        };

        return blobClient.OpenWriteAsync(true, options);
    }

    private static BlobContainerClient GetBlobClient(string container)
    {
        return new BlobContainerClient(EnvironmentConfiguration.GetMandatoryConfiguration("AZURE_STORAGE_CONNECTION_STRING"), container);
    }

    public async Task<bool> ExistsAsync(string container, string id)
    {
        var azureBlobClient = GetBlobClient(container);
        var blobClient = azureBlobClient.GetBlobClient(id);
        var response = await blobClient.ExistsAsync();
        return response.Value;
    }

    public Task<string> GetFileNameAsync(string container, string id)
    {
        var azureBlobClient = GetBlobClient(container);
        var blobClient = azureBlobClient.GetBlobClient(id);
        return blobClient.GetPropertiesAsync().ContinueWith(x => x.Result.Value.Metadata["FileName"]);
    }

    public Task<Stream> OpenFileStreamAsync(string container, string id)
    {
        var azureBlobClient = GetBlobClient(container);
        var blobClient = azureBlobClient.GetBlobClient(id);
        return blobClient.OpenReadAsync();
    }
}