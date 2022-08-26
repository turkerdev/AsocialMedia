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
    private const string ExchangeName = "asocialmedia.upload.exchange";
    
    public QueueManager()
    {
        _factory = new ConnectionFactory { Uri = new Uri(ConfigManager.Get.RabbitMq.Url) };
    }

    public void Connect()
    {
        if (_factory is null)
            throw new Exception("Can't connect to queue. Factory is null");
            
        _connection = _factory.CreateConnection();
        
        _connection.ConnectionBlocked += (_, args) => 
            Logger.Log($"Queue connection blocked: {args.Reason}");
        
        _connection.ConnectionShutdown += (_, args) => 
            Logger.Log($"Queue connection shutdown: {args.ReplyText}");
        
        _connection.ConnectionUnblocked += (_, _) => 
            Logger.Log("Queue connection unblocked");

        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(ExchangeName, "topic", true);
        _channel.BasicQos(0, 1, false);
    }

    public void Subscribe<TConsumerMessage, TConsumer>(string queueName, Func<TConsumerMessage,TConsumer> factory)
        where TConsumerMessage : ConsumerMessage
        where TConsumer : Consumer<TConsumerMessage>
    {
        if (_channel is null)
            throw new Exception("Can't subscribe to queue. Channel is null");
        _channel.QueueDeclare(queueName, true, false, false);
        _channel.QueueBind(queueName, ExchangeName, queueName);

        var eventConsumer = new EventingBasicConsumer(_channel);
        eventConsumer.Received += async (_, args) =>
        {
            Logger.Log("{0}: New message", queueName);
            var bytes = args.Body.ToArray();
            var body = Encoding.UTF8.GetString(bytes);
            var message = JsonConvert.DeserializeObject<TConsumerMessage>(body);

            if (message is null)
                throw new Exception($"{queueName} message deserialization failed, message is null");

            try
            {
                var consumer = factory(message);
                await consumer.Consume();
                consumer.CleanUp();
                _channel.BasicAck(args.DeliveryTag, false);
                Logger.Log("{0}: Successfully handled", queueName);
            }
            catch (Exception e)
            {
                _channel.BasicNack(args.DeliveryTag, false, true);
                Logger.Error("{0}: Error handling message: {1}", queueName, e.Message);
            }
        };
        
        _channel.BasicConsume(queueName, false, eventConsumer);
        Logger.Log("Started consuming {0}", queueName);
    }
}