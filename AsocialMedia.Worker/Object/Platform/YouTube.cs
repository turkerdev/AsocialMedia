using Newtonsoft.Json;

namespace AsocialMedia.Worker.Object.Platform;

internal class YouTube
{
    [JsonProperty(Required = Required.Always)]
    public string AccessToken { get; set; } = string.Empty;

    [JsonProperty(Required = Required.Always)]
    public string RefreshToken { get; set; } = string.Empty;
}
