using AsocialMedia.Worker.DTO;
using Newtonsoft.Json;

namespace AsocialMedia.Worker.Consumer.Compilation
{
    internal class CompilationConsumerMessage
    {
        [JsonProperty(Required = Required.Always)]
        public Destination Destination { get; set; } = new();

        [JsonProperty(Required = Required.Always)]
        public Asset[] Assets { get; set; } = Array.Empty<Asset>();
    }
}
