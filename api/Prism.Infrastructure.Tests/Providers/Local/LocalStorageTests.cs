using System.Security.Cryptography;
using FluentAssertions;
using Prism.Core;
using Prism.Core.Exceptions;
using Prism.Infrastructure.Providers.Local;

namespace Prism.Infrastructure.Tests.Providers.Local;

public class LocalStorageTests
{
    [Fact]
    public async Task All_Test()
    {
        // Arrange
        Environment.SetEnvironmentVariable("STORAGE_DIRECTORY", Path.GetTempPath());
        var data = RandomNumberGenerator.GetBytes(32);
        
        var id = Identifier.GenerateString();
        
        // Act
        var localStorage = new LocalStorage();
        var exists = await localStorage.ExistsAsync("organization", "container", id);
        var stream = await localStorage.CreateFileStreamAsync("organization", "container", "fileName", id);
        await stream.WriteAsync(data);
        await stream.DisposeAsync();
        var fileName = await localStorage.GetFileNameAsync("organization", "container", id);
        stream = await localStorage.OpenFileStreamAsync("organization", "container", id);
        var readData = new byte[32];
        var readAsync = await stream.ReadAsync(readData).ConfigureAwait(false);
        await stream.DisposeAsync();
        await localStorage.DeleteAsync("organization", "container", id);
        
        await Assert.ThrowsAsync<NotFoundException>(async () => await localStorage.GetFileNameAsync("organization", "container", Identifier.GenerateString()));

        // Assert
        exists.Should().BeFalse();
        fileName.Should().Be("fileName");
        readAsync.Should().Be(32);
        readData.Should().BeEquivalentTo(data);
    }
}