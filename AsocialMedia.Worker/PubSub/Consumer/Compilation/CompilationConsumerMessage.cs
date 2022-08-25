using AsocialMedia.Worker.Object;
using Newtonsoft.Json;

namespace AsocialMedia.Worker.PubSub.Consumer.Compilation;

internal class CompilationConsumerMessage
{
    [JsonProperty(Required = Required.Always)]
    public Destination Destination { get; set; } = new();

    [JsonProperty(Required = Required.Always)]
    public Asset[] Assets { get; set; } = Array.Empty<Asset>();
}
