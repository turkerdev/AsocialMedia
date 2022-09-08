using AsocialMedia.Worker.Service;
using FFMpegCore;

namespace AsocialMedia.Worker.PubSub.Consumer.Compilation;

internal class CompilationConsumer : Consumer<CompilationConsumerMessage>
{
    public override async Task Consume(CompilationConsumerMessage message)
    {
        List<string> resources = new();

        foreach (var asset in message.Assets)
        {
            var downloadService = new DownloadService(asset, ResourceGroupId);
            var resourceId = await downloadService.DownloadAsync();
            var resource = AssetManager.GetResource(ResourceGroupId, resourceId);
            resources.Add(resource);
        }

        var outputId = AssetManager.CreateResource();
        var outputPath = AssetManager.GetResourcePathById(ResourceGroupId, outputId);

        FFMpeg.Join(outputPath + ".mp4", resources.ToArray());

        var uploadService = new UploadService(message.Destination, ResourceGroupId, outputId);
        await uploadService.UploadAsync();
    }
}