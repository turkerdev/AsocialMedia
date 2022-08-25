namespace AsocialMedia.Worker.PubSub.Consumer;

public interface IConsumer<T>
{
    string QueueName { get; }
    Task Handle(T message);
}

