using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AsocialMedia.Worker.Helper;

public class Binaries
{
    public const string FFmpegFileName = "ffmpeg.exe";
    public const string FFprobeFileName = "ffprobe.exe";
    public const string YTDLPFileName = "ytdlp.exe";

    public static async Task DownloadFFmpeg()
    {
        var FFmpegExist = File.Exists(FFmpegFileName);
        var FFprobeExist = File.Exists(FFprobeFileName);

        if (FFmpegExist && FFprobeExist)
            return;

        const string zipPath = "ffmpegInstaller.zip";
        const string downloadUrl = "https://github.com/GyanD/codexffmpeg/releases/download/2022-06-27-git-a526f0cc3a/ffmpeg-2022-06-27-git-a526f0cc3a-full_build.zip";
        
        using var wc = new WebClient();

        wc.DownloadProgressChanged += (obj, e) =>
            Console.WriteLine($"FFmpeg binaries downloading: {e.ProgressPercentage}%");

        await wc.DownloadFileTaskAsync(downloadUrl, zipPath);

        using var archive = ZipFile.OpenRead(zipPath);

        var binaries = archive.Entries.Where(file => file.Name == FFmpegFileName || file.Name == FFprobeFileName);
        foreach (var binary in binaries)
            binary.ExtractToFile(Path.Combine(".", binary.Name), true);

        archive.Dispose();
        File.Delete(zipPath);
        Console.WriteLine("FFmpeg downloaded");
    }

    public static async Task DownloadYTDLP()
    {
        var YTDLP = File.Exists(YTDLPFileName);

        if (YTDLP)
            return;

        const string downloadUrl = "https://github.com/yt-dlp/yt-dlp/releases/download/2022.06.29/yt-dlp_min.exe";
        
        using var wc = new WebClient();

        wc.DownloadProgressChanged += (obj, progress) =>
            Console.WriteLine($"YTDLP binaries downloading: {progress.ProgressPercentage}%");

        await wc.DownloadFileTaskAsync(downloadUrl, YTDLPFileName);
        Console.WriteLine(value: "YTDLP downloaded");
    }
}

