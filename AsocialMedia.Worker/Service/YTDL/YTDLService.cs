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
        p.StartInfo.Arguments = $@"{url} --ffmpeg-location ""./{FFmpeg.fileName}"" -f ""best[height<=1080]"" ";
        p.StartInfo.RedirectStandardOutput = true;

        return p;
    }

    public async Task Download(string url, string outputPath, TimeSpan? duration = null, TimeSpan? startTime = null)
    {
        var p = CreateProcess(url);
        if (duration is not null || startTime is not null)
        {
            string args = @"--no-part --downloader ffmpeg --external-downloader-args ""ffmpeg_i:";
            if (duration is not null)
                args += $"-t {duration} ";
            if (startTime is not null)
                args += $"-ss {startTime}";

            p.StartInfo.Arguments += @$"{args}"" ";
        }

        p.StartInfo.Arguments += @$"-o ""{outputPath}""";

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

