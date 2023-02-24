using AsocialMedia.Worker.Object;
using Newtonsoft.Json;

namespace AsocialMedia.Worker.PubSub.Consumer.Shorts;

internal class ShortsConsumerMessage : ConsumerMessage
{
    [JsonProperty(Required = Required.Always)]
    public Asset Asset { get; set; } = new();
}
