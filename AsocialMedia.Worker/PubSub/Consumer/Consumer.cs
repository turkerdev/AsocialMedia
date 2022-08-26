using AsocialMedia.Worker.Object;
using AsocialMedia.Worker.Service.Uploader;

namespace AsocialMedia.Worker.PubSub.Consumer;

public abstract class Consumer<T> where T : ConsumerMessage
{
    protected string AssetDir { get; }
    protected string AssetId { get; }
    protected T Message { get; }

    protected Consumer(T message)
    {
        Message = message;
        var (assetDir,assetId) = AssetManager.CreateOne();
        AssetDir = assetDir;
        AssetId = assetId;
    }

    public void CleanUp()
    {
        AssetManager.DeleteOne(AssetDir);
    }

    public abstract Task Consume();

    public List<IUploaderService> CreateUploadServices(Destination destination, Stream stream)
    {
        var tasks = new List<IUploaderService>();
        
        foreach (var youtube in destination.YouTube)
        {
            var youtubeService = new YouTubeUploaderService();
            youtubeService.Login(youtube.Account);
            youtubeService.CreateVideo(youtube.Video);
            youtubeService.AddSource(stream);

            tasks.Add(youtubeService);
        }
        
        return tasks;
    }
}