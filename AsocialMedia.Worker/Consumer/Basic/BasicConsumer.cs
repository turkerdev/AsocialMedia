using AsocialMedia.Worker.Services;
using Google.Apis.YouTube.v3.Data;

namespace AsocialMedia.Worker.Consumer.Basic;

internal class BasicConsumer : IConsumer<BasicConsumerMessage>
{
    public string queueName => "asocialmedia.upload.basic";

    public async void Handle(BasicConsumerMessage message)
    {
        var directoryName = "assets";
        var exist = Directory.Exists(directoryName);
        if (exist)
            Directory.Delete(directoryName, true);
        Directory.CreateDirectory(directoryName);

        string outputPath = "assets/output.mp4";
        YTDLP.Download(message.Asset.Url, outputPath);

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

        Directory.Delete(directoryName, true);
    }
}

