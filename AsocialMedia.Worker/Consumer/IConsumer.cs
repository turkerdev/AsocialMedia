namespace AsocialMedia.Worker.Consumer;

internal interface IConsumer<T>
{
    string queueName { get; }
    void Handle(T message);
}

