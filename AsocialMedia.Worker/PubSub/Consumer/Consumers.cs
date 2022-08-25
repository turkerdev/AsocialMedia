using AsocialMedia.Worker.PubSub.Consumer.Basic;
using AsocialMedia.Worker.PubSub.Consumer.Compilation;
using AsocialMedia.Worker.PubSub.Consumer.Shorts;

namespace AsocialMedia.Worker.PubSub.Consumer;

internal static class Consumers
{
    public static readonly BasicConsumer Basic = new();
    public static readonly ShortsConsumer Shorts = new();
    public static readonly CompilationConsumer Compilation = new();
}
