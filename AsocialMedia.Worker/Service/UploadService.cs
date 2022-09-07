using AsocialMedia.Worker.Downloader;
using AsocialMedia.Worker.Object;
using AsocialMedia.Worker.Uploader;

namespace AsocialMedia.Worker.Service;

public class UploadService
{
    private List<IBaseUploader> ListUploader { get; } = new();

    public UploadService(Destination destination, string resourcePath)
    {
        foreach (var youtube in destination.YouTube)
        {
            var youtubeService = new YoutubeUploader(youtube.Account, youtube.Video, resourcePath);
            ListUploader.Add(youtubeService);
        }
    }

    public async Task UploadAsync()
    {
        foreach (var uploader in ListUploader)
            await uploader.UploadAsync();
    }
}