using Newtonsoft.Json;

namespace AsocialMedia.Worker.Object.Platform;

internal class YouTube
{
    [JsonProperty(Required = Required.Always)]
    public YouTubeAccount Account = new();
}

internal class YouTubeAccount
{
    [JsonProperty(PropertyName = "access_token", Required = Required.Always)]
    public string AccessToken { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "refresh_token", Required = Required.Always)]
    public string RefreshToken { get; set; } = string.Empty;
}