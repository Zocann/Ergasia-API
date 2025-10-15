namespace Ergasia_API.Services.Interfaces;

public interface IProfilePictureService
{
    public Task<string> UploadAsync(Stream picture, string fileName, string contentType);
}