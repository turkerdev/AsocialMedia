using AsocialMedia.Worker.Helper;
using AsocialMedia.Worker.PubSub;
using AsocialMedia.Worker.PubSub.Consumer;
using AsocialMedia.Worker.PubSub.Consumer.Basic;
using AsocialMedia.Worker.PubSub.Consumer.Compilation;
using AsocialMedia.Worker.PubSub.Consumer.Shorts;

namespace AsocialMedia.Worker;

class Program
{
    private static void Main(string[] args)
    {
        Task.WaitAll(YTDLP.Download(), FFmpeg.Download(), FFprobe.Download());

        AssetManager.Initialize();

        var queueManager = new QueueManager();
        queueManager.Connect();
        queueManager.Subscribe<BasicConsumer, BasicConsumerMessage>("ezupload.upload.basic");
        // queueManager.Subscribe<ShortsConsumerMessage,ShortsConsumer>(
        //     "asocialmedia.upload.shorts", 
        //     (message) => new ShortsConsumer(message)
        // ); 
        // queueManager.Subscribe<CompilationConsumerMessage,CompilationConsumer>(
        //     "asocialmedia.upload.compilation", 
        //     (message) => new CompilationConsumer(message)
        // );

        Logger.Log("Waiting...");
        Thread.Sleep(Timeout.Infinite);
    }
}