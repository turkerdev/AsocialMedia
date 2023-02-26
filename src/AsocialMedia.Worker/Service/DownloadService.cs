using AsocialMedia.Worker.Downloader;
using AsocialMedia.Worker.Object;

namespace AsocialMedia.Worker.Service;

public class DownloadService
{
    private BaseDownloader Downloader { get; }

    public DownloadService(Asset asset, string resourceGroupId)
    {
        Downloader = asset.Url switch
        {
            _ when asset.Url.StartsWith("http") => new YtdlDownloader(asset, resourceGroupId),
            _ => throw new Exception("Unsupported asset type")
        };
    }
    
    public async Task<string> DownloadAsync()
    {
        return await Downloader.DownloadAsync();
    }
}