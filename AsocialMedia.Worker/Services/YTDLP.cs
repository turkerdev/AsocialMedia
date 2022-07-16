using AsocialMedia.Worker.Helper;
using System.Diagnostics;

namespace AsocialMedia.Worker.Services;

public static class YTDLP
{
    private static Process CreateDownloadProcess(string url)
    {
        var p = new Process();
        p.StartInfo.FileName = Helper.YTDLP.fileName;
        p.StartInfo.Arguments = $@"{url} --ffmpeg-location ""./{FFmpeg.fileName}"" -f ""bestvideo[height<=1080]+bestaudio/bestvideo[height<=1080]"" --recode-video mp4 -o ";
        p.StartInfo.RedirectStandardOutput = true;

        return p;
    }

    public static void Download(string url, string outputPath)
    {
        var p = CreateDownloadProcess(url);
        p.StartInfo.Arguments += @$"""{outputPath}""";

        p.Start();
        p.WaitForExit();
    }

    public static StreamReader Download(string url)
    {
        var p = CreateDownloadProcess(url);
        p.StartInfo.Arguments += "-";

        p.Start();

        return p.StandardOutput;
    }
}

