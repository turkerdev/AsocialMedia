using Microsoft.Extensions.Configuration;

namespace AsocialMedia.Worker;

internal static class ConfigManager
{
    private static readonly IConfiguration configRoot;

    static ConfigManager()
    {
        configRoot = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .AddEnvironmentVariables()
            .Build();
    }

    public static Config Get => configRoot.Get<Config>();
}

internal class Config
{
    public GoogleConfig Google { get; init; } = new();
    public RabbitMQConfig RabbitMQ { get; init; } = new();
}

internal class GoogleConfig
{
    public string ClientSecret { get; init; } = string.Empty;
    public string ClientId { get; init; } = string.Empty;
}

internal class RabbitMQConfig
{
    public string URL { get; init; } = string.Empty;
}
