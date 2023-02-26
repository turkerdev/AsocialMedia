using AsocialMedia.Worker.Downloader;
using AsocialMedia.Worker.Object;
using AsocialMedia.Worker.Uploader;

namespace AsocialMedia.Worker.Service;

public class UploadService
{
    private List<IBaseUploader> ListUploader { get; } = new();

    public UploadService(Destination destination, string resourceGroupId, string resourceId)
    {
        foreach (var youtube in destination.YouTube)
        {
            var resource = AssetManager.GetResource(resourceGroupId, resourceId);
            var youtubeService = new YoutubeUploader(youtube.Account, youtube.Video, resource);
            ListUploader.Add(youtubeService);
        }
    }

    public async Task UploadAsync()
    {
        foreach (var uploader in ListUploader)
            await uploader.UploadAsync();
    }
}