using AsocialMedia.Worker.Service.Uploader;
using AsocialMedia.Worker.Service.YTDL;
using FFMpegCore;
using Google.Apis.YouTube.v3.Data;

namespace AsocialMedia.Worker.PubSub.Consumer.Shorts;

internal class ShortsConsumer : Consumer<ShortsConsumerMessage>
{
    public ShortsConsumer(ShortsConsumerMessage message) : base(message)
    {
    }

    public override async Task Consume()
    {
        var ytdlService = new YTDLService();
            
        ytdlService.ProgressChanged += (_, args) =>
            Logger.Log("{0}% of {1}, {2}/s, ~{3}", args.DownloadProgress, args.TotalSize, args.DownloadSpeed, args.ETA);
        
        ytdlService.Downloaded += (_, _) =>
            Logger.Log("{0}: Downloaded", AssetId);
        
        await ytdlService.Download(
            Message.Asset.Url, 
            $"{AssetDir}/raw",
            Message.Asset.StartTime, 
            Message.Asset.EndTime
        );
        
        var rawPath = Directory.GetFiles(AssetDir).Where(file => file.Contains("raw")).First();
        
        await FFMpegArguments.FromFileInput(rawPath)
            .OutputToFile($"{AssetDir}/output", true, opts =>
            {
                opts.WithCustomArgument("-vf scale=720:1280:force_original_aspect_ratio=decrease,pad=720:1280:(ow-iw)/2:(oh-ih)/2,setsar=1");
                opts.ForceFormat("mp4");
            })
            .ProcessAsynchronously();
        
        var outputPath = Directory.GetFiles(AssetDir).Where(file => file.Contains("output")).First();
        await using var fileStream = new FileStream(outputPath, FileMode.Open);
        
        var tasks = new List<IUploaderService>();
        
        foreach (var youtube in Message.Destination.YouTube)
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
