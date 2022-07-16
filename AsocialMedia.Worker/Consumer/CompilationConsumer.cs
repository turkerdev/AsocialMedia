using AsocialMedia.Worker.Services;
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using Google.Apis.YouTube.v3.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace AsocialMedia.Worker.Consumer;

public static class CompilationConsumer
{
    public static async void Consumer(object? sender, BasicDeliverEventArgs e, IModel channel)
    {
        var bodyByte = e.Body.ToArray();
        var body = Encoding.UTF8.GetString(bodyByte);
        var message = JsonSerializer.Deserialize<DTO.CompilationQueue>(body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var directoryName = "assets";
        var exist = Directory.Exists(directoryName);
        if (exist)
            Directory.Delete(directoryName, true);
        Directory.CreateDirectory(directoryName);

        for (int i = 0; i < message.Assets.Count; i++)
        {
            var asset = message.Assets[i];
            var downloadStream = YTDLP.Download(asset.Url);
            FFMpegArguments.FromPipeInput(new StreamPipeSource(downloadStream.BaseStream))
                .OutputToFile($"{directoryName}/{i}.mp4", true, opts =>
                {
                    opts.WithCustomArgument(@"-filter_complex ""[0:v]boxblur=40,scale=720x1280,setsar=1[bg];[0:v]scale=720:1280:force_original_aspect_ratio=decrease[fg];[bg][fg]overlay=y=(H-h)/2""");
                    opts.WithAudioCodec(AudioCodec.Aac);
                })
                .ProcessSynchronously();
        }

        var files = Directory.GetFiles(directoryName);
        var outputPath = $"{directoryName}/output.mp4";
        FFMpeg.Join(outputPath, files);

        var video = new Video();
        video.Snippet = new VideoSnippet();
        video.Snippet.Title = "Default Video Title";
        video.Snippet.Description = "Default Video Description";
        video.Snippet.Tags = new[] { "tag1", "tag2" };
        video.Snippet.CategoryId = "22";
        video.Status = new VideoStatus();
        video.Status.PrivacyStatus = "private";

        await using var fileStream = new FileStream(outputPath, FileMode.Open);

        var youtubeService = Uploader.YouTube.CreateYouTubeService(
            message.Destination.YouTube.AccessToken,
            message.Destination.YouTube.RefreshToken);
        await Uploader.YouTube.Upload(youtubeService, video, fileStream);

        fileStream.Dispose();

        Directory.Delete(directoryName, true);
        channel.BasicAck(e.DeliveryTag, false);
    }
}
