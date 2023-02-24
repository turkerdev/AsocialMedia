using AsocialMedia.Worker.Object;
using Newtonsoft.Json;

namespace AsocialMedia.Worker.PubSub.Consumer.Basic;

public class BasicConsumerMessage : ConsumerMessage
{
    [JsonProperty(Required = Required.Always)]
    public Asset Asset { get; set; } = new();
}

