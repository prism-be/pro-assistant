using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Prism.ProAssistant.Api.Services;

public interface IFileService
{
    Task DeleteFileAsync(string id);
    Task<byte[]?> GetFileAsync(string id);
    Task<string> GetFileNameAsync(string id);
    Task<Stream> GetFileStreamAsync(string id);
    Task<string> UploadFromBytesAsync(string fileName, byte[] bytes);
    Task<string> UploadFromStreamAsync(string fileName, Stream stream);
}

public class FileService : IFileService
{
    private readonly ILogger<FileService> _logger;
    private readonly IUserOrganizationService _userOrganizationService;

    public FileService(ILogger<FileService> logger, IUserOrganizationService userOrganizationService)
    {
        _logger = logger;
        _userOrganizationService = userOrganizationService;
    }

    public async Task<string> UploadFromBytesAsync(string fileName, byte[] bytes)
    {
        _logger.LogInformation("UploadFromBytesAsync - {FileName} - {UserId}", fileName, _userOrganizationService.GetUserId());

        var bucket = await _userOrganizationService.GetUserGridFsBucket();
        var id = await bucket.UploadFromBytesAsync(fileName, bytes);
        return id.ToString();
    }

    public async Task<string> UploadFromStreamAsync(string fileName, Stream stream)
    {
        _logger.LogInformation("UploadFromStreamAsync - {FileName} - {UserId}", fileName, _userOrganizationService.GetUserId());

        var bucket = await _userOrganizationService.GetUserGridFsBucket();
        var id = await bucket.UploadFromStreamAsync(fileName, stream);
        return id.ToString();
    }

    public async Task DeleteFileAsync(string id)
    {
        _logger.LogInformation("DeleteFileAsync - {Id} - {UserId}", id, _userOrganizationService.GetUserId());

        var bucket = await _userOrganizationService.GetUserGridFsBucket();
        await bucket.DeleteAsync(new ObjectId(id));
    }

    public async Task<byte[]?> GetFileAsync(string id)
    {
        _logger.LogInformation("GetFileAsync - {Id} - {UserId}", id, _userOrganizationService.GetUserId());

        var bucket = await _userOrganizationService.GetUserGridFsBucket();
        return await bucket.DownloadAsBytesAsync(new ObjectId(id));
    }

    public async Task<Stream> GetFileStreamAsync(string id)
    {
        _logger.LogInformation("GetFileAsync - {Id} - {UserId}", id, _userOrganizationService.GetUserId());

        var bucket = await _userOrganizationService.GetUserGridFsBucket();
        return await bucket.OpenDownloadStreamAsync(new ObjectId(id));
    }

    public async Task<string> GetFileNameAsync(string id)
    {
        _logger.LogInformation("GetFileNameAsync - {Id} - {UserId}", id, _userOrganizationService.GetUserId());

        var bucket = await _userOrganizationService.GetUserGridFsBucket();
        var result = await bucket.FindAsync(Builders<GridFSFileInfo>.Filter.Eq(x => x.Id, new ObjectId(id)));
        return await result.SingleAsync().ContinueWith(x => x.Result.Filename);
    }
}