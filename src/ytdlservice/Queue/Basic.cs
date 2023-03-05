using ytdlservice.Downloader;

namespace ytdlservice.Queue;

internal record BasicMessage(string Key, string Url)
{
    public string Key { get; init; } = Key;
    public string Url { get; init; } = Url;
    public string Format { get; init; } = "bestvideo*[height<=1080]+bestaudio/best[height<=1080]";
    public string StartTime { get; init; } = "0";
    public string EndTime { get; init; } = "0";
}

internal class Basic : Consumer<BasicMessage>
{
    public override async Task Handle(BasicMessage message)
    {
        await YtdlDownloader.Download(message.Key, message.Url, message.Format, message.StartTime, message.EndTime);
    }
}
