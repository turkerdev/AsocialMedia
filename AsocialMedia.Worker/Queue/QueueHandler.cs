using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AsocialMedia.Worker.Queue;

public class QueueHandler
{
    private readonly string QueueName;
    private readonly IModel Channel;
    private readonly EventingBasicConsumer Consumer;

    public QueueHandler(string queueName, IModel channel, EventHandler<BasicDeliverEventArgs> onConsume)
    {
        QueueName = queueName;
        Channel = channel;

        Channel.QueueDeclare(QueueName, true, false, false);
        Channel.QueueBind(QueueName, "upload.exchange", QueueName);

        Consumer = new(Channel);
        Consumer.Received += onConsume;
    }

    public void Consume()
    {
        Channel.BasicConsume(QueueName, false, Consumer);
    }
}

