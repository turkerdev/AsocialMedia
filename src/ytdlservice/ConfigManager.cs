namespace ytdlservice;

internal static class ConfigManager
{
    public static Env Env { get; } = new();

    static ConfigManager()
    {
        new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build()
            .Bind(Env);
    }
}

internal record Env
{
    public string RABBITMQ_URL { get; init; } = string.Empty;
    public string BUCKET_URL { get; init; } = string.Empty;
    public string BUCKET_KEY { get; init; } = string.Empty;
    public string BUCKET_SECRET { get; init; } = string.Empty;
}