namespace AsocialMedia.Worker.Consumer;

internal interface IConsumer<T>
{
    string queueName { get; }
    Task Handle(T message);
}

