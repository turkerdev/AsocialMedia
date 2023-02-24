using AsocialMedia.Worker.Service;

namespace AsocialMedia.Worker.PubSub.Consumer.Basic;

internal class BasicConsumer : Consumer<BasicConsumerMessage>
{
    public override async Task Consume(BasicConsumerMessage message)
    {
        var downloadService = new DownloadService(message.Asset, ResourceGroupId);
        var resourceId = await downloadService.DownloadAsync();

        var uploadService = new UploadService(message.Destination, ResourceGroupId, resourceId);
        await uploadService.UploadAsync();
    }
}