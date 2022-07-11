using AsocialMedia.Worker.Consumer;
using AsocialMedia.Worker.Helper;
using AsocialMedia.Worker.Queue;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

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

        Task.WaitAll(Binaries.DownloadYTDLP(),Binaries.DownloadFFmpeg());

        var factory = new ConnectionFactory { Uri = new(config.GetSection("RabbitMQ:URL").Value) };
        var connection = factory.CreateConnection();
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
        Console.ReadLine();
    }
}