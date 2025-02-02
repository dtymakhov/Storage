using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using ManagedCode.Storage.AspNetExtensions;
using ManagedCode.Storage.AspNetExtensions.Options;
using ManagedCode.Storage.Core;
using ManagedCode.Storage.Core.Helpers;
using ManagedCode.Storage.Core.Models;
using ManagedCode.Storage.FileSystem.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ManagedCode.Storage.Tests.AspNetExtensions;

public class StorageExtensionsTests
{
    public IStorage Storage { get; }
    public ServiceProvider ServiceProvider { get; }

    public StorageExtensionsTests()
    {
        ServiceProvider = ConfigureServices();
        Storage = ServiceProvider.GetService<IStorage>()!;
    }

    public static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddFileSystemStorageAsDefault(opt =>
        {
            opt.BaseFolder = Path.Combine(Environment.CurrentDirectory, "managed-code-bucket");
        });

        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task UploadToStorageAsync_SmallFile()
    {
        // Arrange
        const int size = 200 * 1024; // 200 KB
        var fileName = FileHelper.GenerateRandomFileName();
        var formFile = FileHelper.GenerateFormFile(fileName, size);

        // Act
        await Storage.UploadToStorageAsync(formFile);
        var localFile = await Storage.DownloadAsync(fileName);

        // Assert
        localFile!.FileInfo.Length.Should().Be(formFile.Length);
        localFile.FileName.Should().Be(formFile.FileName);

        await Storage.DeleteAsync(fileName);
    }

    [Fact]
    public async Task UploadToStorageAsync_LargeFile()
    {
        // Arrange
        const int size = 50 * 1024 * 1024; // 50 MB
        var fileName = FileHelper.GenerateRandomFileName();
        var formFile = FileHelper.GenerateFormFile(fileName, size);

        // Act
        await Storage.UploadToStorageAsync(formFile);
        var localFile = await Storage.DownloadAsync(fileName);

        // Assert
        localFile!.FileInfo.Length.Should().Be(formFile.Length);
        localFile.FileName.Should().Be(formFile.FileName);

        await Storage.DeleteAsync(fileName);
    }

    [Fact]
    public async Task UploadToStorageAsync_WithRandomName()
    {
        // Arrange
        const int size = 200 * 1024; // 200 KB
        var fileName = FileHelper.GenerateRandomFileName();
        var formFile = FileHelper.GenerateFormFile(fileName, size);

        // Act
        var newFileName = await Storage.UploadToStorageAsync(formFile, new UploadToStorageOptions {UseRandomName = true});
        var localFile = await Storage.DownloadAsync(newFileName);

        // Assert
        localFile!.FileInfo.Length.Should().Be(formFile.Length);
        localFile.FileName.Should().Be(newFileName.Name);
        localFile.FileName.Should().NotBe(formFile.FileName);

        await Storage.DeleteAsync(fileName);
    }

    [Fact]
    public async Task DownloadAsFileResult_WithFileName()
    {
        // Arrange
        const int size = 200 * 1024; // 200 KB
        var fileName = FileHelper.GenerateRandomFileName();
        var localFile = FileHelper.GenerateLocalFile(fileName, size);

        // Act
        await Storage.UploadFileAsync(fileName, localFile.FilePath);
        var fileResult = await Storage.DownloadAsFileResult(fileName);

        // Assert
        fileResult!.ContentType.Should().Be(MimeHelper.GetMimeType(localFile.FileInfo.Extension));
        fileResult.FileDownloadName.Should().Be(localFile.FileName);

        await Storage.DeleteAsync(fileName);
    }

    [Fact]
    public async Task DownloadAsFileResult_WithBlobMetadata()
    {
        // Arrange
        const int size = 200 * 1024; // 200 KB
        var fileName = FileHelper.GenerateRandomFileName();
        var localFile = FileHelper.GenerateLocalFile(fileName, size);

        BlobMetadata blobMetadata = new() {Name = fileName};

        // Act
        await Storage.UploadFileAsync(blobMetadata, localFile.FilePath);
        var fileResult = await Storage.DownloadAsFileResult(blobMetadata);

        // Assert
        fileResult!.ContentType.Should().Be(MimeHelper.GetMimeType(localFile.FileInfo.Extension));
        fileResult.FileDownloadName.Should().Be(localFile.FileName);

        await Storage.DeleteAsync(fileName);
    }

    [Fact]
    public async Task DownloadAsFileResult_WithFileName_IfFileDontExist()
    {
        // Arrange
        var fileName = FileHelper.GenerateRandomFileName();

        // Act
        var fileResult = await Storage.DownloadAsFileResult(fileName);

        // Assert
        fileResult.Should().BeNull();
    }

    [Fact]
    public async Task DownloadAsFileResult_WithBlobMetadata_IfFileDontExist()
    {
        // Arrange
        var fileName = FileHelper.GenerateRandomFileName();

        BlobMetadata blobMetadata = new() {Name = fileName};

        // Act
        var fileResult = await Storage.DownloadAsFileResult(blobMetadata);

        // Assert
        fileResult.Should().BeNull();
    }
}