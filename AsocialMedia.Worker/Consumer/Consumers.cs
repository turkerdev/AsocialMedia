using AsocialMedia.Worker.Consumer.Basic;
using AsocialMedia.Worker.Consumer.Compilation;

namespace AsocialMedia.Worker.Consumer;

internal static class Consumers
{
    public static BasicConsumer Basic = new();
    public static CompilationConsumer Compilation = new();
}
