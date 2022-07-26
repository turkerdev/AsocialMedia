using Newtonsoft.Json;

namespace AsocialMedia.Worker.Object;

internal class Asset
{
    [JsonProperty(Required = Required.Always)]
    public string Url { get; set; } = string.Empty;

    public string? Credit { get; set; } = null;

    [JsonProperty(PropertyName = "end_time")]
    public string? EndTime { get; set; } = null;

    [JsonProperty(PropertyName = "start_time")]
    public string? StartTime { get; set; } = null;
}
