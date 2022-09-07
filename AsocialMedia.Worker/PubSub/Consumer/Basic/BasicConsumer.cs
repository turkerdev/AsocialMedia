using AsocialMedia.Worker.Service.Uploader;
using AsocialMedia.Worker.Service.YTDL;
using Google.Apis.YouTube.v3.Data;

namespace AsocialMedia.Worker.PubSub.Consumer.Basic;

internal class BasicConsumer : Consumer<BasicConsumerMessage>
{
    public override async Task Consume(BasicConsumerMessage message)
    {
        var resourceId = AssetManager.CreateResource();
        var resourcePath = AssetManager.GetResourceById(ResourceGroupId, resourceId);
        
        var ytdlService = new YTDLService();

        await ytdlService.Download(
            message.Asset.Url, 
            resourcePath,
            message.Asset.StartTime, 
            message.Asset.EndTime
        );

        await using var fileStream = new FileStream(resourcePath, FileMode.Open);

        var tasks = new List<IUploaderService>();
        
        foreach (var youtube in message.Destination.YouTube)
        {
            var youtubeService = new YouTubeUploaderService();
            youtubeService.Login(youtube.Account);
            youtubeService.CreateVideo(youtube.Video);
            youtubeService.AddSource(fileStream);

            tasks.Add(youtubeService);
        }
        
        foreach (var task in tasks)
            await task.UploadVideoAsync();
    }
}
