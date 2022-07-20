namespace AsocialMedia.Worker.Service.YTDL;

public class YTDLProgressChanged
{
    public float DownloadProgress { get; set; }
    public string TotalSize { get; set; } = string.Empty;
    public string DownloadSpeed { get; set; } = string.Empty;
    public TimeSpan ETA { get; set; }
}

