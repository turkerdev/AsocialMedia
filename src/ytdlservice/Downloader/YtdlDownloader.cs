using System.Diagnostics;
using ytdlservice.Queue.Message;

namespace ytdlservice.Downloader;

public class YtdlDownloader
{
    public Stream Stream(VideoDownload message)
    {
        var process = new Process();
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.FileName = "ytdlp";
        process.StartInfo.ArgumentList.Add(message.Url);
        process.StartInfo.ArgumentList.Add("-f");
        process.StartInfo.ArgumentList.Add(message.Format);
        process.StartInfo.ArgumentList.Add("--remux-video");
        process.StartInfo.ArgumentList.Add("mp4");
        process.StartInfo.ArgumentList.Add("-o");
        process.StartInfo.ArgumentList.Add("-");

        if (message.StartTime is not null || message.EndTime is not null)
        {
            var startTime = message.StartTime ?? "0";
            var endTime = message.EndTime ?? "0";
            process.StartInfo.ArgumentList.Add("--download-sections");
            process.StartInfo.ArgumentList.Add($"*{startTime}-{endTime}");
        }

        process.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);

        process.Start();
        process.BeginErrorReadLine();

        return process.StandardOutput.BaseStream;
    }
}
