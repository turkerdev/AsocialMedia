using Newtonsoft.Json;

namespace AsocialMedia.Worker.Object;

internal class Asset
{
    [JsonProperty(Required = Required.Always)]
    public string Url { get; set; } = string.Empty;

    public string? Credit { get; set; } = null;
    public TimeSpan? Duration { get; set; } = null;

    [JsonProperty(PropertyName = "start_time")]
    public TimeSpan? StartTime { get; set; } = null;
}
