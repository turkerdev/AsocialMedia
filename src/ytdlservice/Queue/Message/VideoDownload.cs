namespace ytdlservice.Queue.Message;

public record VideoDownload(string Id, string Url)
{
    public string Id { get; init; } = Id;
    public string Url { get; init; } = Url;
    public string Format { get; init; } = "best[height=1080]/bestvideo*[height<=1080]+bestaudio";
    public string? StartTime { get; init; }
    public string? EndTime { get; init; }
}
