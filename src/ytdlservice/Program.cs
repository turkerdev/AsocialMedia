using MassTransit;
using ytdlservice.Queue.Consumer;
using Amazon.S3;
using Microsoft.Extensions.Options;
using ytdlservice.Uploader;
using ytdlservice.Downloader;
using ytdlservice;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<Env>(new ConfigurationBuilder().AddEnvironmentVariables().Build());
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<Env>>().Value);

        services.AddSingleton<IAmazonS3>(sp =>
        {
            var env = sp.GetRequiredService<IOptions<Env>>().Value;
            Console.WriteLine(env);
            var s3Config = new AmazonS3Config
            {
                ServiceURL = env.S3_URL,
                ForcePathStyle = true,
            };

            var client = new AmazonS3Client(env.S3_KEY, env.S3_SECRET, s3Config);

            return client;
        });

        services.AddSingleton<YtdlDownloader>();
        services.AddSingleton<S3Uploader>();

        services.AddMassTransit(bus =>
        {
            bus.AddConsumer<VideoDownloadConsumer, VideoDownloadDefinition>();

            bus.UsingRabbitMq((ctx, cfg) =>
            {
                var env = ctx.GetRequiredService<IOptions<Env>>().Value;
                cfg.Host(new Uri(env.RABBITMQ_URL));
                cfg.UseConcurrencyLimit(1);
                cfg.PrefetchCount = 1;
                cfg.ClearSerialization();
                cfg.AddRawJsonSerializer();

                cfg.ConfigureEndpoints(ctx);
            });

        });
    })
    .Build();

await host.StartAsync();
await host.WaitForShutdownAsync();