using AsocialMedia.Worker.Helper;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AsocialMedia.Worker.Service.YTDL;

public class YTDLService
{
    public event EventHandler<YTDLProgressChanged>? ProgressChanged;
    public event EventHandler? Downloaded;

    private static Process CreateDownloadProcess(string url)
    {
        var p = new Process();
        p.StartInfo.FileName = YTDLP.fileName;
        p.StartInfo.Arguments = $@"{url} --ffmpeg-location ""./{FFmpeg.fileName}"" -f ""bestvideo[height<=1080]+bestaudio/bestvideo[height<=1080]"" -o ";
        p.StartInfo.RedirectStandardOutput = true;

        return p;
    }

    public async Task Download(string url, string outputPath)
    {
        var p = CreateDownloadProcess(url);
        p.StartInfo.Arguments += @$"""{outputPath}""";

        p.OutputDataReceived += (sender, e) =>
        {
            if (e.Data is null)
                return;

            var pattern = new Regex(@"^\[download]\s+(\d+\.\d)% of ((?:\d+\.\d+)(?:K|M|B|G)iB) at\s+((?:\d+\.\d+)(?:K|M|B|G)iB)\/s ETA (\d+:\d+)$");

            var match = pattern.Match(e.Data);
            if (!match.Success)
                return;

            var progress = new YTDLProgressChanged();
            progress.DownloadProgress = float.Parse(match.Groups[1].Value);
            progress.TotalSize = match.Groups[2].Value;
            progress.DownloadSpeed = match.Groups[3].Value;
            progress.ETA = TimeSpan.ParseExact(match.Groups[4].Value, @"mm\:ss", null);

            ProgressChanged?.Invoke(this, progress);
        };

        p.Start();
        p.BeginOutputReadLine();
        await p.WaitForExitAsync();

        Downloaded?.Invoke(this, EventArgs.Empty);
    }
}

