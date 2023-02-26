using AsocialMedia.Worker.PubSub;
using AsocialMedia.Worker.PubSub.Consumer.Basic;
using AsocialMedia.Worker.PubSub.Consumer.Compilation;
using AsocialMedia.Worker.PubSub.Consumer.Shorts;

namespace AsocialMedia.Worker;

class Program
{
    private static void Main(string[] args)
    {
        var queueManager = new QueueManager();
        queueManager.Connect();
        queueManager.Subscribe<BasicConsumer, BasicConsumerMessage>(
            "ezupload.upload.basic");
        queueManager.Subscribe<ShortsConsumer, ShortsConsumerMessage>(
            "ezupload.upload.shorts"); 
        queueManager.Subscribe<CompilationConsumer, CompilationConsumerMessage>(
            "ezupload.upload.compilation");

        Logger.Log("Waiting...");
        Thread.Sleep(Timeout.Infinite);
    }
}