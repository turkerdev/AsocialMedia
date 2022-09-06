// using AsocialMedia.Worker.Service.Uploader;
// using AsocialMedia.Worker.Service.YTDL;
// using FFMpegCore;
// using Google.Apis.YouTube.v3.Data;
//
// namespace AsocialMedia.Worker.PubSub.Consumer.Compilation;
//
// internal class CompilationConsumer : Consumer<CompilationConsumerMessage>
// {
//     public CompilationConsumer(CompilationConsumerMessage message) : base(message)
//     {
//     }
//
//     public override async Task Consume()
//     {
//         for (var i = 0; i < Message.Assets.Length; i++)
//         {
//             var asset = Message.Assets[i];
//             
//             var ytdlService = new YTDLService();
//             
//             ytdlService.ProgressChanged += (_, args) =>
//                 Logger.Log("{0}% of {1}, {2}/s, ~{3}", args.DownloadProgress, args.TotalSize, args.DownloadSpeed, args.ETA);
//         
//             ytdlService.Downloaded += (_, _) =>
//                 Logger.Log("{0}: Downloaded", AssetId);
//             
//             var assetPath = $"{AssetDir}/asset_{i}";
//             await ytdlService.Download(asset.Url, assetPath, asset.StartTime, asset.EndTime);
//             
//             if (asset.Credit is not null)
//             {
//                 Logger.Log("{0}: {1} Adding credit to video", AssetId, i);
//
//                 var assetTempPath = $"{AssetDir}/asset_temp_{i}";
//                 
//                 await FFMpegArguments.FromFileInput(assetPath)
//                     .OutputToFile(assetTempPath, true, opts =>
//                     {
//                         opts.WithAudioCodec("copy");
//                         opts.Resize(1280, 720);
//                         opts.WithCustomArgument($"-vf drawtext=text='{asset.Credit}':fontcolor=white:fontsize=24:x=w-tw-10:y=10:box=1:boxcolor=black@0.5:boxborderw=5");
//                         opts.ForceFormat("mp4");
//                     })
//                     .ProcessAsynchronously();
//
//                 File.Move(assetTempPath, assetPath, true);
//                 Logger.Log("{0}: {1} Credit added", AssetId, i);
//             }
//         }
//
//         var files = Directory.GetFiles(AssetDir);
//         var assets = files.Where(file => file.Contains("asset"));
//         FFMpeg.Join($"{AssetDir}/output.mp4", assets.ToArray());
//
//         await using var fileStream = new FileStream($"{AssetDir}/output.mp4", FileMode.Open);
//
//         var tasks = new List<IUploaderService>();
//         
//         foreach (var youtube in Message.Destination.YouTube)
//         {
//             var youtubeService = new YouTubeUploaderService();
//             youtubeService.Login(youtube.Account);
//             youtubeService.CreateVideo(youtube.Video);
//             youtubeService.AddSource(fileStream);
//
//             tasks.Add(youtubeService);
//         }
//         
//         foreach (var task in tasks)
//             await task.UploadVideoAsync();
//     }
// }
