using AsocialMedia.Worker.Consumer;
using AsocialMedia.Worker.Queue;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using AsocialMedia.Worker.Helper;

namespace AsocialMedia.Worker;

class Program
{
    public static IConfiguration config;

    private static void Main(string[] args)
    {
        config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .AddEnvironmentVariables()
            .Build();

        Task.WaitAll(YTDLP.Download(),FFmpeg.Download());

        var factory = new ConnectionFactory { Uri = new(config.GetSection("RabbitMQ:URL").Value) };
        var connection = factory.CreateConnection();
        connection.ConnectionBlocked += (sender, e) => Console.WriteLine($"Queue connection blocked: {e.Reason}");
        connection.ConnectionShutdown += (sender, e) => Console.WriteLine($"Queue connection shutdown: {e.ReplyText}");
        connection.ConnectionUnblocked += (sender, e) => Console.WriteLine($"Queue connection unblocked");
        var channel = connection.CreateModel();

        channel.ExchangeDeclare("upload.exchange", "topic", true);
        channel.BasicQos(0, 1, false);

        var basicQueue = new QueueHandler(
            "upload.basic",
            channel, 
            (sender,e) => BasicConsumer.Consumer(sender, e, channel));
        basicQueue.Consume();

        var compilationQueue = new QueueHandler(
            "upload.compilation",
            channel, 
            (sender,e) => CompilationConsumer.Consumer(sender,e,channel));
        compilationQueue.Consume();

        Console.WriteLine("Waiting...");
        Thread.Sleep(Timeout.Infinite);
    }
}