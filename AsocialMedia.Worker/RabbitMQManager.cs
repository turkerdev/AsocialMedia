using AsocialMedia.Worker.Consumer;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace AsocialMedia.Worker;

public static class RabbitMQManager
{
    private static IModel Channel { get; set; }

    static RabbitMQManager()
    {
        var factory = new ConnectionFactory { Uri = new(ConfigManager.Get.RabbitMQ.URL) };
        var connection = factory.CreateConnection();

        connection.ConnectionBlocked += (sender, e) => Console.WriteLine($"Queue connection blocked: {e.Reason}");
        connection.ConnectionShutdown += (sender, e) => Console.WriteLine($"Queue connection shutdown: {e.ReplyText}");
        connection.ConnectionUnblocked += (sender, e) => Console.WriteLine($"Queue connection unblocked");

        Channel = connection.CreateModel();

        Channel.ExchangeDeclare("asocialmedia.upload.exchange", "topic", true);
        Channel.BasicQos(0, 1, false);
    }

    internal static void AddConsumer<T>(IConsumer<T> consumer)
    {
        Channel.QueueDeclare(consumer.queueName, true, false, false);
        Channel.QueueBind(consumer.queueName, "asocialmedia.upload.exchange", consumer.queueName);

        var eventConsumer = new EventingBasicConsumer(Channel);

        eventConsumer.Received += (_, e) =>
        {
            var bodyByte = e.Body.ToArray();
            var body = Encoding.UTF8.GetString(bodyByte);
            var message = JsonConvert.DeserializeObject<T>(body);

            if (message is null)
                throw new Exception("Queue incoming message can not be null");

            consumer.Handle(message);
            Channel.BasicAck(e.DeliveryTag, false);
        };

        Channel.BasicConsume(consumer.queueName, false, eventConsumer);
    }
}

