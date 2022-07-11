namespace AsocialMedia.Worker.DTO;

public class Destination
{
    public YouTubeCredentials YouTube { get; set; } = new();
}

public class YouTubeCredentials
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

public class Asset
{
    public string Url { get; set; } = string.Empty;
}
