using AsocialMedia.Worker.DTO;
using Newtonsoft.Json;

namespace AsocialMedia.Worker.Consumer.Basic;

internal class BasicConsumerMessage
{
    [JsonProperty(Required = Required.Always)]
    public Destination Destination { get; set; } = new();

    [JsonProperty(Required = Required.Always)]
    public Asset Asset { get; set; } = new();
}

