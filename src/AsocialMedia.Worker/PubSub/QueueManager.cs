using System.Text;
using AsocialMedia.Worker.PubSub.Consumer;
using AsocialMedia.Worker.PubSub.Consumer.Basic;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AsocialMedia.Worker.PubSub;

public class QueueManager
{
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;
    private IModel? _channel;
    private const string ExchangeUploadDirect = "ezupload.upload.direct";

    public QueueManager()
    {
        _factory = new ConnectionFactory { Uri = new Uri(ConfigManager.Get.RabbitMq.Url) };
    }

    public void Connect()
    {
        _connection = _factory.CreateConnection();

        _connection.ConnectionBlocked += (_, args) =>
            Logger.Log($"Queue connection blocked: {args.Reason}");

        _connection.ConnectionShutdown += (_, args) =>
            Logger.Log($"Queue connection shutdown: {args.ReplyText}");

        _connection.ConnectionUnblocked += (_, _) =>
            Logger.Log("Queue connection unblocked");

        _channel = _connection.CreateModel();
        _channel.BasicQos(0, 1, true);
        _channel.ExchangeDeclare(ExchangeUploadDirect, "direct", true);
        _channel.ExchangeDeclare(ExchangeUploadDirect + ".dead", "direct", true);
    }

    public void Subscribe<TConsumer, TConsumerMessage>(string queueName)
        where TConsumerMessage : ConsumerMessage
        where TConsumer : Consumer<TConsumerMessage>, new()
    {
        if (_channel is null)
            throw new Exception("Can't subscribe to queue. Channel is null");

        // Dead letter queue
        _channel.QueueDeclare(queueName + ".dead", true, false, false);
        _channel.QueueBind(queueName + ".dead", ExchangeUploadDirect + ".dead", queueName + ".dead");

        // Regular queue
        var args = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", ExchangeUploadDirect + ".dead" },
            { "x-dead-letter-routing-key", queueName + ".dead" }
        };

        _channel.QueueDeclare(queueName, true, false, false, args);
        _channel.QueueBind(queueName, ExchangeUploadDirect, queueName);

        var eventConsumer = new EventingBasicConsumer(_channel);

        eventConsumer.Received += async (_, deliverArgs) =>
            await EventConsumerOnReceived<TConsumer, TConsumerMessage>(deliverArgs, queueName);

        _channel.BasicConsume(queueName, false, eventConsumer);
        Logger.Log($"Started consuming {queueName}");
    }

    private async Task EventConsumerOnReceived<TConsumer, TConsumerMessage>(BasicDeliverEventArgs deliverArgs, string queueName)
        where TConsumerMessage : ConsumerMessage
        where TConsumer : Consumer<TConsumerMessage>, new()
    {
        if (_channel is null)
            throw new Exception("Cannot consume the message. Channel is null");
        
        Logger.Log($"[{queueName}]: New message received");
        try
        {
            await Consume<TConsumer, TConsumerMessage>(deliverArgs);
            _channel.BasicAck(deliverArgs.DeliveryTag, false);
            Logger.Log($"[{queueName}]: Successfully acknowledged");
        }
        catch (Exception e)
        {
            _channel.BasicReject(deliverArgs.DeliveryTag, false);
            Logger.Log($"[{queueName}]: Rejected: '{e.Message}'");
        }
    }

    private T ParseMessage<T>(ReadOnlyMemory<byte> memory) where T : ConsumerMessage
    {
        var bytes = memory.ToArray();
        var body = Encoding.UTF8.GetString(bytes);
        var message = JsonConvert.DeserializeObject<T>(body);

        if (message is null)
            throw new Exception("Failed to deserialize the message");

        return message;
    }

    private async Task Consume<TConsumer, TConsumerMessage>(BasicDeliverEventArgs deliverArgs)
        where TConsumerMessage : ConsumerMessage
        where TConsumer : Consumer<TConsumerMessage>, new()
    {
        using var consumer = new TConsumer();
        var message = ParseMessage<TConsumerMessage>(deliverArgs.Body);
        await consumer.Consume(message);
    }
}