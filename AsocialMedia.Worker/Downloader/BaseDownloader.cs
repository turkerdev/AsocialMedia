namespace AsocialMedia.Worker.Downloader;

public abstract class BaseDownloader
{
    protected string ResourceId { get; }
    
    protected BaseDownloader()
    {
        ResourceId = AssetManager.CreateResource();
    }
    
    public abstract Task<string> DownloadAsync();
}