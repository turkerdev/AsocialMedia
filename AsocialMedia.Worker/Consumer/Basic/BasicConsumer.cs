using AsocialMedia.Worker.Service.YTDL;
using Google.Apis.YouTube.v3.Data;

namespace AsocialMedia.Worker.Consumer.Basic;

internal class BasicConsumer : IConsumer<BasicConsumerMessage>
{
    public string queueName => "asocialmedia.upload.basic";


    public async Task Handle(BasicConsumerMessage message)
    {
        var directoryName = Guid.NewGuid().ToString();
        var directory = $"assets/{directoryName}";
        Directory.CreateDirectory(directory);
        Console.WriteLine("Using {0} for {1}", directoryName, queueName);

        var ytdlService = new YTDLService();

        ytdlService.ProgressChanged += (_, args) =>
        {
            Console.WriteLine("{0}% of {1}, {2}/s, ~{3}", args.DownloadProgress, args.TotalSize, args.DownloadSpeed, args.ETA);
        };

        ytdlService.Downloaded += (_, _) =>
        {
            Console.WriteLine("{0}: Downloaded", directoryName);
        };

        await ytdlService.Download(message.Asset.Url, $"{directory}/output", message.Asset.StartTime, message.Asset.EndTime);

        var outputPath = Directory.GetFiles(directory).Where(file => file.Contains("output")).First();
        using var fileStream = new FileStream(outputPath, FileMode.Open);

        var tasks = new List<Task>();

        foreach (var youtube in message.Destination.YouTube)
        {
            var youtubeService = Uploader.YouTube.CreateYouTubeService(
                youtube.Account.AccessToken,
                youtube.Account.RefreshToken);

            var video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = youtube.Title;
            if (youtube.Description is not null)
                video.Snippet.Description = youtube.Description;
            if (youtube.Tags is not null)
                video.Snippet.Tags = youtube.Tags;
            video.Snippet.CategoryId = "22";
            video.Status = new VideoStatus();
            video.Status.MadeForKids = youtube.MadeForKids;
            video.Status.PrivacyStatus = youtube.Privacy;
            if (youtube.PublishAt is not null)
                video.Status.PublishAtRaw = youtube.PublishAt;


            var task = Task.Run(() => Uploader.YouTube.Upload(youtubeService, video, fileStream));
            tasks.Add(task);
        }

        Task.WaitAll(tasks.ToArray());

        fileStream.Dispose();
        Directory.Delete(directory, true);
        Console.WriteLine("{0}: Done", directoryName);
    }
}
