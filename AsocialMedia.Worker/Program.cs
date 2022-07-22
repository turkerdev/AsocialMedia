using AsocialMedia.Worker.Consumer;
using AsocialMedia.Worker.Helper;

namespace AsocialMedia.Worker;

class Program
{

    private static void Main(string[] args)
    {
        Task.WaitAll(YTDLP.Download(), FFmpeg.Download(), FFprobe.Download());

        var isExist = Directory.Exists("assets");
        if (isExist)
            Directory.Delete("assets", true);
        Directory.CreateDirectory("assets");

        RabbitMQManager.AddConsumer(Consumers.Basic);
        RabbitMQManager.AddConsumer(Consumers.Shorts);

        Console.WriteLine("Waiting...");
        Thread.Sleep(Timeout.Infinite);
    }
}