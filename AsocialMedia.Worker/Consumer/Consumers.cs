using AsocialMedia.Worker.Consumer.Basic;
using AsocialMedia.Worker.Consumer.Compilation;
using AsocialMedia.Worker.Consumer.Shorts;

namespace AsocialMedia.Worker.Consumer;

internal static class Consumers
{
    public static BasicConsumer Basic = new();
    public static ShortsConsumer Shorts = new();
    public static CompilationConsumer Compilation = new();
}
