using AsocialMedia.Worker.Service;
using AsocialMedia.Worker.Service.Uploader;
using Google.Apis.YouTube.v3.Data;

namespace AsocialMedia.Worker.PubSub.Consumer.Basic;

internal class BasicConsumer : Consumer<BasicConsumerMessage>
{
    public override async Task Consume(BasicConsumerMessage message)
    {
        var downloadService = new DownloadService(message.Asset, ResourceGroupId);
        var resourceId = await downloadService.DownloadAsync();
        var resourcePath = AssetManager.GetResourceById(ResourceGroupId, resourceId);

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