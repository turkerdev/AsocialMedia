using AsocialMedia.Worker.Service;
using FFMpegCore;

namespace AsocialMedia.Worker.PubSub.Consumer.Shorts;

internal class ShortsConsumer : Consumer<ShortsConsumerMessage>
{
    public override async Task Consume(ShortsConsumerMessage message)
    {
        var downloadService = new DownloadService(message.Asset, ResourceGroupId);
        var resourceId = await downloadService.DownloadAsync();
        var resource = AssetManager.GetResource(ResourceGroupId, resourceId);

        var outputId = AssetManager.CreateResource();
        var outputPath = AssetManager.GetResourcePathById(ResourceGroupId, outputId);

        await FFMpegArguments
            .FromFileInput(resource)
            .OutputToFile(outputPath, true, opts => opts
                .WithCustomArgument("-vf crop=in_w*0.6:in_h:in_w*0.2:0,pad=iw:ih*16/9:(ow-iw)/2:(oh-ih)/2")
                .ForceFormat("mp4"))
            .ProcessAsynchronously();

        var uploadService = new UploadService(message.Destination, ResourceGroupId, outputId);
        await uploadService.UploadAsync();
    }
}