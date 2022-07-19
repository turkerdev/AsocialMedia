using AsocialMedia.Worker.Services;
using Google.Apis.YouTube.v3.Data;

namespace AsocialMedia.Worker.Consumer.Basic;

internal class BasicConsumer : IConsumer<BasicConsumerMessage>
{
    public string queueName => "asocialmedia.upload.basic";

    public async void Handle(BasicConsumerMessage message)
    {
        var directoryName = Guid.NewGuid().ToString();
        var directory = $"assets/{directoryName}";
        Directory.CreateDirectory(directory);
        Console.WriteLine("Using {0} for {1}", directoryName, queueName);

        string outputPath = $"{directory}/output.mp4";
        YTDLP.Download(message.Asset.Url, outputPath);
        Console.WriteLine("{0}: Downloaded", directoryName);

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
        Console.WriteLine("{0}: Done", directoryName);
    }
}

