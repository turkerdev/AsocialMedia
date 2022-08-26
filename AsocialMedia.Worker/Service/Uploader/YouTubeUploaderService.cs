using AsocialMedia.Worker.Object.Platform;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace AsocialMedia.Worker.Service.Uploader;

public class YouTubeUploaderService : IUploaderService
{
    private YouTubeService? _youtubeService;
    private Video? _video;
    private Stream? _stream;
    
    public async Task UploadVideoAsync()
    {
        if(_video is null  || _youtubeService is null || _stream is null)
            throw new Exception("No video or stream or youtube service found");

        var videosInsertRequest = _youtubeService.Videos.Insert(_video, "snippet,status", _stream, "video/*");
        videosInsertRequest.ChunkSize = ResumableUpload.MinimumChunkSize;

        videosInsertRequest.ProgressChanged += (progress) =>
        {
            switch (progress.Status)
            {
                case UploadStatus.Starting:
                    Logger.Log("YouTube: Starting to upload.");
                    break;
                case UploadStatus.Uploading:
                    Logger.Log("YouTube: {0} bytes sent.", progress.BytesSent);
                    break;
                case UploadStatus.Completed:
                    Logger.Log("YouTube: Uploaded.");
                    break;
                case UploadStatus.Failed:
                    Logger.Log("YouTube: An error prevented the upload from completing.\n{0}", progress.Exception);
                    break;
                case UploadStatus.NotStarted:
                    Logger.Log("YouTube: The upload has not started.");
                    break;
            }
        };

        await videosInsertRequest.UploadAsync();
    }

    public void AddSource(Stream stream)
    {
        _stream = stream;
    }
    
    public void CreateVideo(VideoYouTube video)
    {
        var snippet = new VideoSnippet
        {
            Title = video.Title,
            Description = video.Description,
            Tags = video.Tags,
            CategoryId = video.CategoryId,
        };

        var status = new VideoStatus
        {
            MadeForKids = video.MadeForKids,
            PrivacyStatus = video.Privacy,
            PublishAtRaw = video.PublishAt
        };
        
        var v = new Video
        {
            Snippet = snippet,
            Status = status
        };

        _video = v;
    }
    
    public void Login(AccountYouTube account)
    {
        var secrets = new ClientSecrets
        {
            ClientId = ConfigManager.Get.Google.ClientId,
            ClientSecret = ConfigManager.Get.Google.ClientSecret
        };

        var scopes = new[]
        {
            YouTubeService.Scope.Youtube,
            YouTubeService.Scope.YoutubeUpload
        };
        
        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = secrets,
            Scopes = scopes
        });

        var token = new TokenResponse
        {
            AccessToken = account.AccessToken,
            RefreshToken = account.RefreshToken
        };

        var credential = new UserCredential(flow, "", token);

        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
        });

        _youtubeService = youtubeService;
    }
}