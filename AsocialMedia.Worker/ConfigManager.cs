using Microsoft.Extensions.Configuration;

namespace AsocialMedia.Worker;

internal static class ConfigManager
{
    private static readonly IConfiguration ConfigRoot;

    static ConfigManager()
    {
        ConfigRoot = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .AddEnvironmentVariables()
            .Build();
    }

    public static Config Get => ConfigRoot.Get<Config>();
}

internal class Config
{
    public GoogleConfig Google { get; init; } = new();
    public RabbitMqConfig RabbitMq { get; init; } = new();
}

internal class GoogleConfig
{
    public string ClientSecret { get; init; } = string.Empty;
    public string ClientId { get; init; } = string.Empty;
}

internal class RabbitMqConfig
{
    public string Url { get; init; } = string.Empty;
}
