using Newtonsoft.Json;

namespace AsocialMedia.Worker.Object.Platform;

public class PlatformYouTube
{
    [JsonProperty(Required = Required.Always)]
    public AccountYouTube Account = new();

    [JsonProperty(Required = Required.Always)]
    public VideoYouTube Video = new();
}

public class VideoYouTube
{
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

public class AccountYouTube
{
    [JsonProperty(PropertyName = "access_token", Required = Required.Always)]
    public string AccessToken { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "refresh_token", Required = Required.Always)]
    public string RefreshToken { get; set; } = string.Empty;
}