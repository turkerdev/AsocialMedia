namespace ytdlservice;

public record Env
{
    public string RABBITMQ_URL { get; init; } = string.Empty;
    public string S3_URL { get; init; } = string.Empty;
    public string S3_KEY { get; init; } = string.Empty;
    public string S3_SECRET { get; init; } = string.Empty;
    public string API_URL { get; init; } = string.Empty;
}

public record Config
{
    public static string S3_BUCKET { get; } = "app";
}