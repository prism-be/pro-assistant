using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Prism.Core;

namespace Prism.Infrastructure.Providers.Azure;

public class BlobDataStorage : IDataStorage
{
    public Task<Stream> CreateFileStreamAsync(string organization, string container, string fileName, string id)
    {
        var azureBlobClient = GetBlobClient(organization);
        var fqId = $"{container}/{id}";
        var blobClient = azureBlobClient.GetBlobClient(fqId);

        var options = new BlobOpenWriteOptions
        {
            Metadata = new Dictionary<string, string>
            {
                { "FileName", fileName }
            }
        };

        return blobClient.OpenWriteAsync(true, options);
    }

    public async Task<bool> ExistsAsync(string organization, string container, string id)
    {
        var azureBlobClient = GetBlobClient(organization);
        var fqId = $"{container}/{id}";
        var blobClient = azureBlobClient.GetBlobClient(fqId);
        var response = await blobClient.ExistsAsync();
        return response.Value;
    }

    public Task<string> GetFileNameAsync(string organization, string container, string id)
    {
        var azureBlobClient = GetBlobClient(organization);
        var fqId = $"{container}/{id}";
        var blobClient = azureBlobClient.GetBlobClient(fqId);
        return blobClient.GetPropertiesAsync().ContinueWith(x => x.Result.Value.Metadata["FileName"]);
    }

    public Task<Stream> OpenFileStreamAsync(string organization, string container, string id)
    {
        var azureBlobClient = GetBlobClient(organization);
        var fqId = $"{container}/{id}";
        var blobClient = azureBlobClient.GetBlobClient(fqId);
        return blobClient.OpenReadAsync();
    }

    public Task DeleteAsync(string organization, string container, string id)
    {
        var azureBlobClient = GetBlobClient(organization);
        var fqId = $"{container}/{id}";
        var blobClient = azureBlobClient.GetBlobClient(fqId);
        return blobClient.DeleteAsync();
    }

    private static BlobContainerClient GetBlobClient(string organization)
    {
        return new BlobContainerClient(
            EnvironmentConfiguration.GetMandatoryConfiguration("AZURE_STORAGE_CONNECTION_STRING"), organization);
    }
}