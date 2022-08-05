using Newtonsoft.Json;

namespace AsocialMedia.Worker.Object.Platform;

internal class YouTube
{
    [JsonProperty(Required = Required.Always)]
    public YouTubeAccount Account = new();

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = null;
    public string[]? Tags { get; set; } = null;
    public string Privacy { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "category")]
    public string? CategoryId { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "made_for_kids")]
    public bool MadeForKids { get; set; } = false;

    [JsonProperty(PropertyName = "publish_at")]
    public string? PublishAt { get; set; } = null;
}

internal class YouTubeAccount
{
    [JsonProperty(PropertyName = "access_token", Required = Required.Always)]
    public string AccessToken { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "refresh_token", Required = Required.Always)]
    public string RefreshToken { get; set; } = string.Empty;
}