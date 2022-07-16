using Newtonsoft.Json;

namespace AsocialMedia.Worker.DTO;

public class Destination
{
    [JsonProperty(Required = Required.Always)] // Currently, its required
    public YouTubeCredentials YouTube { get; set; } = new();
}

public class YouTubeCredentials
{
    [JsonProperty(Required = Required.Always)]
    public string AccessToken { get; set; } = string.Empty;
    [JsonProperty(Required = Required.Always)]
    public string RefreshToken { get; set; } = string.Empty;
}

public class Asset
{
    [JsonProperty(Required = Required.Always)]
    public string Url { get; set; } = string.Empty;
}
