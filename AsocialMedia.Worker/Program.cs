using AsocialMedia.Worker.Consumer;
using AsocialMedia.Worker.Helper;

namespace AsocialMedia.Worker;

class Program
{

    private static void Main(string[] args)
    {
        Task.WaitAll(YTDLP.Download(), FFmpeg.Download(), FFprobe.Download());

        RabbitMQManager.AddConsumer(Consumers.Basic);
        RabbitMQManager.AddConsumer(Consumers.Compilation);

        Console.WriteLine("Waiting...");
        Thread.Sleep(Timeout.Infinite);
    }
}