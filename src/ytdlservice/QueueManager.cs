using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ytdlservice.Queue;

internal class QueueManager
{
    private IModel Channel { get; init; }

    public QueueManager()
    {
        var Factory = new ConnectionFactory
        {
            Uri = new Uri(ConfigManager.Env.RABBITMQ_URL)
        };

        var connection = Factory.CreateConnection();
        Channel = connection.CreateModel();
        Channel.BasicQos(0, 1, true);
    }

    public void Subscribe<Consumer, ConsumerMessage>(string QueueName)
        where Consumer : Consumer<ConsumerMessage>, new()
    {
        Channel.QueueDeclare(QueueName, true, false, false);
        Channel.QueueBind(QueueName, "amq.direct", QueueName);
        var eventConsumer = new EventingBasicConsumer(Channel);

        eventConsumer.Received += async (_, deliverArgs) =>
        {
            Console.WriteLine($"INFO: Received message from '{QueueName}'");
            try
            {
                var Consumer = new Consumer();
                var Message = Consumer.Parse(Encoding.UTF8.GetString(deliverArgs.Body.ToArray()));
                Console.WriteLine($"INFO: {Message}");
                await Consumer.Handle(Message);
                Channel.BasicAck(deliverArgs.DeliveryTag, false);
            }
            catch (Exception e)
            {
                Channel.BasicReject(deliverArgs.DeliveryTag, false);
                Console.WriteLine($"ERROR: {e.Message}");
            }
        };

        Channel.BasicConsume(QueueName, false, eventConsumer);
    }
}