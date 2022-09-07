using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AsocialMedia.Worker.Service.YTDL;

public class YTDLService
{
    private static Process CreateProcess(string url)
    {
        var p = new Process();
        p.StartInfo.FileName = "ytdlp";
        p.StartInfo.ArgumentList.Add(url);
        p.StartInfo.ArgumentList.Add("-f bestvideo*[height<=1080]+bestaudio/best[height<=1080]");
        p.StartInfo.ArgumentList.Add("--no-part");
        p.StartInfo.RedirectStandardOutput = true;

        p.OutputDataReceived += (_, args) =>
        {
            if (args.Data is not null)
                Logger.Log(args.Data);
        };
        
        return p;
    }

    public async Task Download(string url, string outputPath, string? startTime = null, string? endTime = null)
    {
        var p = CreateProcess(url);
        
        if (endTime is not null || startTime is not null)
        {
            var start = startTime ?? "0";
            var end = endTime ?? "0";
            p.StartInfo.ArgumentList.Add($"--download-sections \"*{start}-{end}\"");
        }

        p.StartInfo.ArgumentList.Add($"-o{outputPath}");

        p.Start();
        p.BeginOutputReadLine();
        await p.WaitForExitAsync();
    }
}