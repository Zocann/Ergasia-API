using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Ergasia_API.Services.Interfaces;

namespace Ergasia_API.Services;

public class ProfilePictureService : IProfilePictureService
{
    private readonly BlobContainerClient _containerClient;

    public ProfilePictureService(IConfiguration configuration)
    {
        var connectionString = GetConnectionString(configuration);
        if (string.IsNullOrEmpty(connectionString)) throw new Exception($"{nameof(ProfilePictureService)} connection string is null or empty");

        var containerName = GetContainerName(configuration);
        if (string.IsNullOrEmpty(connectionString)) throw new Exception($"{nameof(ProfilePictureService)} container name is null or empty");
        
        _containerClient = new BlobContainerClient(connectionString, containerName);
        _containerClient.CreateIfNotExists();
    }
    
    public async Task<string> UploadAsync(Stream picture, string fileName, string contentType)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);
        var blobHeaders = BuildBlobHeaders(contentType);
        var blobOptions = BuildBlobOptions(blobHeaders);
        
        await blobClient.UploadAsync(picture, blobOptions);
        
        return blobClient.Uri.ToString();
    }

    private string? GetConnectionString(IConfiguration configuration)
    {
        return Environment.GetEnvironmentVariable("STORAGE_CONNECTION_STRING");
        //return configuration.GetConnectionString("StorageConnection");
    }

    private string? GetContainerName(IConfiguration configuration)
    {
        return configuration["AzureStorage:ProfilePictures"];
    }

    private BlobHttpHeaders BuildBlobHeaders(string contentType)
    {
        return new BlobHttpHeaders
        {
            ContentType = contentType
        };
    }

    private BlobUploadOptions BuildBlobOptions(BlobHttpHeaders headers)
    {
        return new BlobUploadOptions
        {
            HttpHeaders = headers
        };
    }
}