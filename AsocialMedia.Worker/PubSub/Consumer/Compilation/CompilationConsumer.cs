// using AsocialMedia.Worker.Service;
// using FFMpegCore;
//
// namespace AsocialMedia.Worker.PubSub.Consumer.Compilation;
//
// internal class CompilationConsumer : Consumer<CompilationConsumerMessage>
// {
//     public override async Task Consume(CompilationConsumerMessage message)
//     {
//         List<string> resourcePaths = new();
//
//         foreach (var asset in message.Assets)
//         {
//             var downloadService = new DownloadService(asset, ResourceGroupId);
//             var resourceId = await downloadService.DownloadAsync();
//             var resourcePath = AssetManager.GetResourceById(ResourceGroupId, resourceId);
//             resourcePaths.Add(resourcePath);
//         }
//
//         var outputResourceId = AssetManager.CreateResource();
//         var outputResourcePath = AssetManager.GetResourceById(ResourceGroupId, outputResourceId);
//
//         FFMpeg.Join(outputResourcePath+".mp4", resourcePaths.ToArray());
//
//         var uploadService = new UploadService(message.Destination, outputResourcePath);
//         await uploadService.UploadAsync();
//
//         //     if (asset.Credit is not null)
//         //     {
//         //         Logger.Log("{0}: {1} Adding credit to video", AssetId, i);
//         //
//         //         var assetTempPath = $"{AssetDir}/asset_temp_{i}";
//         //         
//         //         await FFMpegArguments.FromFileInput(assetPath)
//         //             .OutputToFile(assetTempPath, true, opts =>
//         //             {
//         //                 opts.WithAudioCodec("copy");
//         //                 opts.Resize(1280, 720);
//         //                 opts.WithCustomArgument($"-vf drawtext=text='{asset.Credit}':fontcolor=white:fontsize=24:x=w-tw-10:y=10:box=1:boxcolor=black@0.5:boxborderw=5");
//         //                 opts.ForceFormat("mp4");
//         //             })
//         //             .ProcessAsynchronously();
//         //
//         //         File.Move(assetTempPath, assetPath, true);
//         //         Logger.Log("{0}: {1} Credit added", AssetId, i);
//         //     }
//     }
// }