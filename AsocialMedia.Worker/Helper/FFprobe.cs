using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;

namespace AsocialMedia.Worker.Helper;

public static class FFprobe
{
    public static readonly string fileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "ffprobe.exe" : "ffprobe";
    private static readonly string downloadUrl = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        ? "https://github.com/ffbinaries/ffbinaries-prebuilt/releases/download/v4.4.1/ffprobe-4.4.1-win-64.zip"
        : "https://github.com/ffbinaries/ffbinaries-prebuilt/releases/download/v4.4.1/ffmpeg-4.4.1-linux-64.zip";
    private static bool isExist => File.Exists(fileName);

    public static async Task Download()
    {
        if (isExist)
            return;

        using var wc = new WebClient();

        wc.DownloadProgressChanged += (obj, e) =>
            Console.WriteLine($"{fileName} downloading: {e.ProgressPercentage}%");

        var zipPath = $"{fileName}.zip";
        await wc.DownloadFileTaskAsync(downloadUrl, zipPath);
        Console.WriteLine($"{fileName} downloaded");

        using var archive = ZipFile.OpenRead(zipPath);
        var entry = archive.GetEntry(fileName);
        if (entry == null)
            throw new Exception($@"{fileName} cannot be found in ""{zipPath}""");
        entry.ExtractToFile(fileName);
        archive.Dispose();
        File.Delete(zipPath);

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