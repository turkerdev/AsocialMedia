using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace AsocialMedia.Worker.Uploader;

public static class YouTube
{
    public static YouTubeService CreateYouTubeService(string accessToken, string refreshToken)
    {
        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = ConfigManager.Get.Google.ClientId,
                ClientSecret = ConfigManager.Get.Google.ClientSecret
            },
            Scopes = new[] {
                YouTubeService.Scope.Youtube,
                YouTubeService.Scope.YoutubeUpload
            },
        }); ;

        var token = new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        var credential = new UserCredential(flow, "", token);

        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
        });

        return youtubeService;
    }

    public static async Task Upload(YouTubeService youtubeService, Video video, Stream stream)
    {
        var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", stream, "video/*");
        videosInsertRequest.ChunkSize = ResumableUpload.MinimumChunkSize;

        videosInsertRequest.ProgressChanged += (progress) =>
        {
            switch (progress.Status)
            {
                case UploadStatus.Starting:
                    Console.WriteLine("YouTube: Starting to upload.");
                    break;
                case UploadStatus.Uploading:
                    Console.WriteLine("YouTube: {0} bytes sent.", progress.BytesSent);
                    break;
                case UploadStatus.Completed:
                    Console.WriteLine("YouTube: Uploaded.");
                    break;
                case UploadStatus.Failed:
                    Console.WriteLine("YouTube: An error prevented the upload from completing.\n{0}", progress.Exception);
                    break;
                case UploadStatus.NotStarted:
                    Console.WriteLine("YouTube: The upload has not started.");
                    break;
            }
        };

        await videosInsertRequest.UploadAsync();
    }
}
