using System.Diagnostics;
using AsocialMedia.Worker.Object;

namespace AsocialMedia.Worker.Downloader;

public class YtdlDownloader : BaseDownloader
{
    private Process _process;

    public YtdlDownloader(Asset asset, string resourceGroupId)
    {
        Logger.Log("{0}: New resource", AssetManager.GetResourcePathById(resourceGroupId, ResourceId));
        _process = new Process();
        _process.StartInfo.RedirectStandardOutput = true;
        _process.StartInfo.FileName = "ytdlp";
        _process.StartInfo.ArgumentList.Add(asset.Url);
        _process.StartInfo.ArgumentList.Add("-f bestvideo*[height<=1080]+bestaudio/best[height<=1080]");
        _process.StartInfo.ArgumentList.Add("--no-part");
        _process.StartInfo.ArgumentList.Add("--concurrent-fragments");
        _process.StartInfo.ArgumentList.Add((Math.Max(1, Process.GetCurrentProcess().Threads.Count - 1)).ToString());
#if DEBUG
        _process.StartInfo.ArgumentList.Add("-v");
#endif
        if (asset.EndTime is not null || asset.StartTime is not null)
        {
            var start = asset.StartTime ?? "0";
            var end = asset.EndTime ?? "0";
            _process.StartInfo.ArgumentList.Add("--download-sections");
            _process.StartInfo.ArgumentList.Add($"*{start}-{end}");
        }

        _process.StartInfo.ArgumentList.Add("-o");
        _process.StartInfo.ArgumentList.Add(AssetManager.GetResourcePathById(resourceGroupId, ResourceId));

        _process.OutputDataReceived += (_, args) =>
        {
            if (args.Data is not null)
                Logger.Log(args.Data);
        };
    }

    public override async Task<string> DownloadAsync()
    {
        Logger.Log("{0}: Downloading", ResourceId);
        _process.Start();
        _process.BeginOutputReadLine();
        await _process.WaitForExitAsync();
        Logger.Log("{0}: Downloaded", ResourceId);

        return ResourceId;
    }
}