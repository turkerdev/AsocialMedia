using AsocialMedia.Worker.Services;
using FFMpegCore;
using FFMpegCore.Pipes;
using Google.Apis.YouTube.v3.Data;

namespace AsocialMedia.Worker.Consumer.Compilation;

internal class CompilationConsumer : IConsumer<CompilationConsumerMessage>
{
    public string queueName => "asocialmedia.upload.compilation";

    public async void Handle(CompilationConsumerMessage message)
    {
        var directoryName = Guid.NewGuid().ToString();
        var directory = $"assets/{directoryName}";
        Directory.CreateDirectory(directory);

        for (int i = 0; i < message.Assets.Length; i++)
        {
            var asset = message.Assets[i];
            var downloadStream = YTDLP.Download(asset.Url);
            FFMpegArguments.FromPipeInput(new StreamPipeSource(downloadStream.BaseStream))
                .OutputToFile($"{directory}/{i}.mp4", true, opts =>
                {
                    opts.WithCustomArgument(@"-filter_complex ""[0:v]boxblur=40,scale=720x1280,setsar=1[bg];[0:v]scale=720:1280:force_original_aspect_ratio=decrease[fg];[bg][fg]overlay=y=(H-h)/2""");
                })
                .ProcessSynchronously();
        }

        var files = Directory.GetFiles(directory);
        var outputPath = $"{directory}/output.mp4";
        FFMpeg.Join(outputPath, files);

        var video = new Video();
        video.Snippet = new VideoSnippet();
        video.Snippet.Title = "Default Video Title";
        video.Snippet.Description = "Default Video Description";
        video.Snippet.Tags = new[] { "tag1", "tag2" };
        video.Snippet.CategoryId = "22";
        video.Status = new VideoStatus();
        video.Status.PrivacyStatus = "private";

        await using var fileStream = new FileStream(outputPath, FileMode.Open);

        var youtubeService = Uploader.YouTube.CreateYouTubeService(
            message.Destination.YouTube.AccessToken,
            message.Destination.YouTube.RefreshToken);
        await Uploader.YouTube.Upload(youtubeService, video, fileStream);

        fileStream.Dispose();

        Directory.Delete(directory, true);
    }
}
