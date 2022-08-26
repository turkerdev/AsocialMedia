namespace AsocialMedia.Worker.Service.Uploader;

public interface IUploaderService
{
    void AddSource(Stream stream);
    Task UploadVideoAsync();
}