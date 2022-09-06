using AsocialMedia.Worker.Service.Uploader;
using AsocialMedia.Worker.Service.YTDL;
using Google.Apis.YouTube.v3.Data;

namespace AsocialMedia.Worker.PubSub.Consumer.Basic;

internal class BasicConsumer : Consumer<BasicConsumerMessage>
{
    public override async Task Consume(BasicConsumerMessage message)
    {
        var ytdlService = new YTDLService();

        ytdlService.ProgressChanged += (_, args) =>
            Logger.Log("{0}% of {1}, {2}/s, ~{3}", args.DownloadProgress, args.TotalSize, args.DownloadSpeed, args.ETA);
        
        ytdlService.Downloaded += (_, _) =>
            Logger.Log("{0}: Downloaded", AssetId);

        await ytdlService.Download(
            message.Asset.Url, 
            $"{AssetDir}/output",
            message.Asset.StartTime, 
            message.Asset.EndTime
        );

        var outputPath = Directory.GetFiles(AssetDir).First(file => file.Contains("output"));
        await using var fileStream = new FileStream(outputPath, FileMode.Open);

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
