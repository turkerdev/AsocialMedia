using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;

namespace AsocialMedia.Worker.Helper;

public static class FFmpeg
{
    public static readonly string fileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "ffmpeg.exe" : "ffmpeg";
    private static readonly string downloadUrl = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        ? "https://github.com/eugeneware/ffmpeg-static/releases/download/b5.0.1/win32-x64"
        : "https://github.com/eugeneware/ffmpeg-static/releases/download/b5.0.1/linux-x64";
    private static bool isExist => File.Exists(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? fileName : $"/bin/{fileName}");

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
            File.Move(fileName, $"/bin/{fileName}");
        }
    }
}