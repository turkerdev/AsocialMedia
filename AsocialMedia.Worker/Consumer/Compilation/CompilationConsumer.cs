using AsocialMedia.Worker.Services;
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
            Console.WriteLine("{0}: Downloading {1}/{2}", directoryName, i + 1, message.Assets.Length);
            var asset = message.Assets[i];

            var rawPath = $"{directory}/raw_{i}.mp4";

            YTDLP.Download(asset.Url, rawPath);
            await FFMpegArguments.FromFileInput(rawPath)
                .OutputToFile($"{directory}/ready_to_concat_{i}.mp4", true, opts =>
                {
                    opts.WithCustomArgument(@"-filter_complex ""[0:v]boxblur=40,scale=720x1280,setsar=1[bg];[0:v]scale=720:1280:force_original_aspect_ratio=decrease[fg];[bg][fg]overlay=y=(H-h)/2""");
                })
                .ProcessAsynchronously();

            File.Delete(rawPath);
            Console.WriteLine("{0}: Downloaded {1}/{2}", directoryName, i + 1, message.Assets.Length);
        }

        var allFiles = Directory.GetFiles(directory);
        var mergeableFiles = allFiles.Where(x => x.StartsWith("ready_to_concat"));
        var outputPath = $"{directory}/output.mp4";
        FFMpeg.Join(outputPath, mergeableFiles.ToArray());

        var video = new Video();
        video.Snippet = new VideoSnippet();
        video.Snippet.Title = "Default Video Title";
        video.Snippet.Description = "Default Video Description";
        video.Snippet.Tags = new[] { "tag1", "tag2" };
        video.Snippet.CategoryId = "22";
        video.Status = new VideoStatus();
        video.Status.PrivacyStatus = "private";

        using var fileStream = new FileStream(outputPath, FileMode.Open);

        var youtubeService = Uploader.YouTube.CreateYouTubeService(
            message.Destination.YouTube.AccessToken,
            message.Destination.YouTube.RefreshToken);
        await Uploader.YouTube.Upload(youtubeService, video, fileStream);

        fileStream.Dispose();

        Directory.Delete(directory, true);
        Console.WriteLine("{0}: Done", directoryName);
    }
}
