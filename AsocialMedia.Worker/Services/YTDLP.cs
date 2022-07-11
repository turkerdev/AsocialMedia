using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsocialMedia.Worker.Services;

public static class YTDLP
{
    private static Process CreateDownloadProcess(string url)
    {
        var p = new Process();
        p.StartInfo.FileName = Helper.Binaries.YTDLPFileName;
        p.StartInfo.Arguments = $@"{url} -f ""bestvideo[height<=1080]+bestaudio/bestvideo[height<=1080]"" --recode-video mp4 -o ";
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.RedirectStandardInput = true; // FIX: Do i need them?
        #if (!DEBUG)
        p.StartInfo.CreateNoWindow = true;
        #endif

        return p;
    }

    public static void Download(string url, string outputPath)
    {
        var p = CreateDownloadProcess(url);
        p.StartInfo.Arguments += @$"""{outputPath}""";

        p.Start();
        p.StandardInput.Dispose();
        p.WaitForExit();
    }

    public static StreamReader Download(string url)
    {
        var p = CreateDownloadProcess(url);
        p.StartInfo.Arguments += "-";

        p.Start();
        p.StandardInput.Dispose();

        return p.StandardOutput;
    }
}

