using AsocialMedia.Worker.Helper;
using AsocialMedia.Worker.PubSub;
using AsocialMedia.Worker.PubSub.Consumer;

namespace AsocialMedia.Worker;

class Program
{
    private static void Main(string[] args)
    {
        Task.WaitAll(YTDLP.Download(), FFmpeg.Download(), FFprobe.Download());

        AssetManager.Initialize();

        var queueManager = new QueueManager();
        queueManager.Connect();
        queueManager.Subscribe(Consumers.Basic);
        queueManager.Subscribe(Consumers.Shorts);
        queueManager.Subscribe(Consumers.Compilation);
        
        Logger.Log("Waiting...");
        Thread.Sleep(Timeout.Infinite);
    }
}