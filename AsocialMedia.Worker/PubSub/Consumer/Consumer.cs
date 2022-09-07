using AsocialMedia.Worker.Object;

namespace AsocialMedia.Worker.PubSub.Consumer;

public abstract class Consumer<T> : IDisposable
    where T : ConsumerMessage
{
    protected string ResourceGroupId { get; }

    protected Consumer()
    {
        ResourceGroupId = AssetManager.CreateResourceGroup();
    }

    public void Dispose()
    {
        AssetManager.DeleteResourceGroupById(ResourceGroupId);
    }

    public abstract Task Consume(T message);
}