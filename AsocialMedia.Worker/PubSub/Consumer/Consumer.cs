using AsocialMedia.Worker.Object;
using AsocialMedia.Worker.Service.Uploader;

namespace AsocialMedia.Worker.PubSub.Consumer;

public abstract class Consumer<T> : IDisposable where T : ConsumerMessage
{
    protected string AssetDir { get; }
    protected string AssetId { get; }

    protected Consumer()
    {
        var (assetDir, assetId) = AssetManager.CreateOne();
        AssetDir = assetDir;
        AssetId = assetId;
    }

    public void Dispose()
    {
        AssetManager.DeleteOne(AssetDir);
    }

    public abstract Task Consume(T message);

    // public List<IUploaderService> CreateUploadServices(Destination destination, Stream stream)
    // {
    //     var tasks = new List<IUploaderService>();
    //
    //     foreach (var youtube in destination.YouTube)
    //     {
    //         var youtubeService = new YouTubeUploaderService();
    //         youtubeService.Login(youtube.Account);
    //         youtubeService.CreateVideo(youtube.Video);
    //         youtubeService.AddSource(stream);
    //
    //         tasks.Add(youtubeService);
    //     }
    //
    //     return tasks;
    // }
}