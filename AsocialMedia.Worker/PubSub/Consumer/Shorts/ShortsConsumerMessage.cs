using AsocialMedia.Worker.Object;
using Newtonsoft.Json;

namespace AsocialMedia.Worker.PubSub.Consumer.Shorts;

internal class ShortsConsumerMessage
{
    [JsonProperty(Required = Required.Always)]
    public Destination Destination { get; set; } = new();

    [JsonProperty(Required = Required.Always)]
    public Asset Asset { get; set; } = new();
}
