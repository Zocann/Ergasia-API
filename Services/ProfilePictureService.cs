using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Ergasia_API.Services.Interfaces;

namespace Ergasia_API.Services;

public class ProfilePictureService : IProfilePictureService
{
    private readonly BlobContainerClient _containerClient;

    public ProfilePictureService(IConfiguration configuration)
    {
        var connectionString = Environment.GetEnvironmentVariable("STORAGE_CONNECTION_STRING");
        //var connectionString = configuration.GetConnectionString("StorageConnection");
        var containerName = configuration["AzureStorage:ProfilePictures"];
        _containerClient = new BlobContainerClient(connectionString, containerName);
        _containerClient.CreateIfNotExists();
    }
    
    public async Task<string> UploadAsync(Stream picture, string fileName, string contentType)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);
        
        var headers = new BlobHttpHeaders
        {
            ContentType = contentType
        };
        
        await blobClient.UploadAsync(picture, new BlobUploadOptions
        {
            HttpHeaders = headers
        });
        
        return blobClient.Uri.ToString();
    }
}