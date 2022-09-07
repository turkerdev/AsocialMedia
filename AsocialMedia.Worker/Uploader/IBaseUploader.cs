namespace AsocialMedia.Worker.Uploader;

public interface IBaseUploader
{
    public Task UploadAsync();
}