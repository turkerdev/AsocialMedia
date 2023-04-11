using Amazon.S3;
using MassTransit;
using ytdlservice.Downloader;
using ytdlservice.Queue.Message;
using ytdlservice.Uploader;

namespace ytdlservice.Queue.Consumer;

public class VideoDownloadDefinition : ConsumerDefinition<VideoDownloadConsumer>
{
    public VideoDownloadDefinition()
    {
        EndpointName = "video.download";
    }
}

public class VideoDownloadConsumer : IConsumer<VideoDownload>
{
    private readonly S3Uploader _s3Uploader;
    private readonly YtdlDownloader _ytdlDownloader;

    public VideoDownloadConsumer(S3Uploader s3Uploader, YtdlDownloader ytdlDownloader)
    {
        _s3Uploader = s3Uploader;
        _ytdlDownloader = ytdlDownloader;
    }

    public async Task Consume(ConsumeContext<VideoDownload> ctx)
    {
        var stream = _ytdlDownloader.Stream(ctx.Message);
        await _s3Uploader.StreamAsync(ctx.Message, stream);
        Console.WriteLine("done");
    }
}