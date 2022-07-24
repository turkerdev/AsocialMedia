using AsocialMedia.Worker.Service.YTDL;
using FFMpegCore;
using Google.Apis.YouTube.v3.Data;

namespace AsocialMedia.Worker.Consumer.Compilation;

internal class CompilationConsumer : IConsumer<CompilationConsumerMessage>
{
    public string queueName => "asocialmedia.upload.compilation";

    public async Task Handle(CompilationConsumerMessage message)
    {
        var directoryName = Guid.NewGuid().ToString();
        var directory = $"assets/{directoryName}";
        Directory.CreateDirectory(directory);
        Console.WriteLine("Using {0} for {1}", directoryName, queueName);

        for (int i = 0; i < message.Assets.Length; i++)
        {
            var asset = message.Assets[i];
            var ytdlService = new YTDLService();

            ytdlService.ProgressChanged += YTDL_ProgressChanged;
            ytdlService.Downloaded += (_, _) => YTDL_Downloaded(directoryName);

            var assetPath = $"{directory}/asset_{i}";
            await ytdlService.Download(asset.Url, assetPath);

            if (asset.Metadata?.Credit is not null)
            {
                Console.WriteLine("{0}: Adding credit to video", directoryName);

                await FFMpegArguments.FromFileInput(assetPath)
                    .OutputToFile($"{directory}/asset_temp_{i}", true, opts =>
                    {
                        opts.WithAudioCodec("copy");
                        opts.Resize(1280, 720);
                        opts.WithCustomArgument($"-vf drawtext=text='{asset.Metadata.Credit}':fontcolor=white:fontsize=24:x=w-tw-10:y=10:box=1:boxcolor=black@0.5:boxborderw=5");
                        opts.ForceFormat("mp4");

                    })
                    .ProcessAsynchronously();

                File.Move($"{directory}/asset_temp_{i}", $"{directory}/asset_{i}", true);
                Console.WriteLine("{0}: Credit added", directoryName);
            }

        }


        var files = Directory.GetFiles(directory);
        var assets = files.Where(x => x.StartsWith($"{directory}\\asset"));
        FFMpeg.Join($"{directory}/output.mp4", assets.ToArray());

        var video = new Video();
        video.Snippet = new VideoSnippet();
        video.Snippet.Title = "Default Video Title";
        video.Snippet.Description = "Default Video Description";
        video.Snippet.Tags = new[] { "tag1", "tag2" };
        video.Snippet.CategoryId = "22";
        video.Status = new VideoStatus();
        video.Status.PrivacyStatus = "private";

        using var fileStream = new FileStream($"{directory}/output.mp4", FileMode.Open);

        var tasks = new List<Task>();

        foreach (var youtube in message.Destination.YouTube)
        {
            var youtubeService = Uploader.YouTube.CreateYouTubeService(
                youtube.AccessToken,
                youtube.RefreshToken);

            var task = Task.Run(() => Uploader.YouTube.Upload(youtubeService, video, fileStream));
            tasks.Add(task);
        }

        Task.WaitAll(tasks.ToArray());

        fileStream.Dispose();
        Directory.Delete(directory, true);
        Console.WriteLine("{0}: Done", directoryName);
    }

    private void YTDL_Downloaded(string directoryName)
    {
        Console.WriteLine("{0}: Downloaded", directoryName);
    }

    private void YTDL_ProgressChanged(object? sender, YTDLProgressChanged args)
    {
        Console.WriteLine("{0}% of {1}, {2}/s, ~{3}", args.DownloadProgress, args.TotalSize, args.DownloadSpeed, args.ETA);
    }

}
