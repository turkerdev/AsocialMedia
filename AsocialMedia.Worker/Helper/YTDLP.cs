using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;

namespace AsocialMedia.Worker.Helper;

public static class YTDLP
{
    public static readonly string fileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "ytdlp.exe" : "ytdlp";
    private static readonly string downloadUrl = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        ? "https://github.com/yt-dlp/yt-dlp/releases/download/2022.06.29/yt-dlp_min.exe"
        : "https://github.com/yt-dlp/yt-dlp/releases/download/2022.06.29/yt-dlp_linux";
    private static bool isExist => File.Exists(fileName);

    public static async Task Download()
    {
        if (isExist)
            return;

        using var wc = new WebClient();

        wc.DownloadProgressChanged += (obj, e) =>
            Console.WriteLine($"{fileName} downloading: {e.ProgressPercentage}%");
        await wc.DownloadFileTaskAsync(downloadUrl, fileName);
        Console.WriteLine($"{fileName} downloaded");

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var p = new Process();
            p.StartInfo.FileName = "/bin/bash";
            p.StartInfo.Arguments = @$"-c ""chmod +x {fileName}""";
            p.Start();
            await p.WaitForExitAsync();
        }
    }
}