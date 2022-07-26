using AsocialMedia.Worker.Helper;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AsocialMedia.Worker.Service.YTDL;

public class YTDLService
{
    public event EventHandler<YTDLProgressChanged>? ProgressChanged;
    public event EventHandler? Downloaded;

    private static Process CreateProcess(string url)
    {
        var p = new Process();
        p.StartInfo.FileName = YTDLP.fileName;
        p.StartInfo.Arguments = $@"{url}";
        p.StartInfo.Arguments += @" -f ""bestvideo[height<=1080]*+bestaudio/best[height<=1080]""";
        p.StartInfo.Arguments += " --no-part";
        p.StartInfo.RedirectStandardOutput = true;

        return p;
    }

    public async Task Download(string url, string outputPath, TimeSpan? startTime = null, TimeSpan? endTime = null)
    {
        var p = CreateProcess(url);
        if (endTime is not null || startTime is not null)
        {
            var start = startTime.ToString() ?? "0";
            var end = endTime.ToString() ?? "0";
            var args = $@" --download-sections ""*{start}-{end}""";
            p.StartInfo.Arguments += @$" {args}";
        }

        p.StartInfo.Arguments += @$" -o ""{outputPath}""";

        p.OutputDataReceived += (sender, e) =>
        {
            if (e.Data is null)
                return;

            var pattern = new Regex(@"^\[download]\s+(\d+\.\d)% of ((?:\d+\.\d+)(?:K|M|B|G)iB) at\s+((?:\d+\.\d+)(?:K|M|B|G)iB)\/s ETA (\d+:\d+)$");

            var match = pattern.Match(e.Data);
            if (!match.Success)
            {
                Console.WriteLine("[UNPARSED]: " + e.Data);
                return;
            }

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

