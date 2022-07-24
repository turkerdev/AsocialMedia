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

        var downloadTasks = new List<Task>();

        for (int i = 0; i < message.Assets.Length; i++)
        {
            var ytdlService = new YTDLService();

            ytdlService.ProgressChanged += (_, args) =>
            {
                Console.WriteLine("{0}% of {1}, {2}/s, ~{3}", args.DownloadProgress, args.TotalSize, args.DownloadSpeed, args.ETA);
            };

            ytdlService.Downloaded += (_, _) =>
            {
                Console.WriteLine("{0}: Downloaded", directoryName);
            };

            await ytdlService.Download(message.Assets[i].Url, $"{directory}/asset_{i}");
        }

        var assets = Directory.GetFiles(directory).Where(x => x.Contains("asset"));
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
}
