using Amazon.S3;
using Amazon.S3.Model;
using ytdlservice.Queue.Message;

namespace ytdlservice.Uploader;

public class S3Uploader
{
    private readonly IAmazonS3 _s3;
    private readonly Env _env;

    public S3Uploader(Env env, IAmazonS3 s3)
    {
        _env = env;
        _s3 = s3;
        Console.WriteLine("if i see it, it works! :)");
        Console.WriteLine(env);
    }

    public async Task StreamAsync(VideoDownload message, Stream stream)
    {
        var multipartRequest = new InitiateMultipartUploadRequest
        {
            BucketName = Config.S3_BUCKET,
            Key = message.Id + ".mp4",
            ContentType = "video/mp4"
        };

        var multipart = await _s3.InitiateMultipartUploadAsync(multipartRequest);

        var partNumber = 1;
        var parts = new List<UploadPartResponse>();
        byte[] buffer = new byte[1024 * 1024 * 5];
        int bufferPosition = 0;
        bool isEnd = false;

        while (isEnd == false)
        {
            //read until buffer is full, or end of stream
            while (bufferPosition < buffer.Length && isEnd == false)
            {
                var readBytes = await stream.ReadAsync(buffer, bufferPosition, buffer.Length - bufferPosition);
                bufferPosition += readBytes;
                isEnd = readBytes == 0;
            }

            //upload the part
            if (isEnd)
            {
                Console.Write("Last part, ");
            }
            Console.WriteLine($"Upload part {partNumber} with {bufferPosition} bytes");
            var uploadPartRequest = new UploadPartRequest
            {
                BucketName = multipart.BucketName,
                Key = multipart.Key,
                UploadId = multipart.UploadId,
                // This is a workaround since i can't use self-signed certificates
                DisablePayloadSigning = _env.S3_URL.Contains("https"),
                DisableMD5Stream = true,
                PartNumber = partNumber,
                PartSize = bufferPosition,
                InputStream = new MemoryStream(buffer, 0, bufferPosition)
            };
            var uploadPartResponse = await _s3.UploadPartAsync(uploadPartRequest);
            parts.Add(uploadPartResponse);
            partNumber++;

            //reset the buffer
            Array.Clear(buffer);
            bufferPosition = 0;
        }

        var completeRequest = new CompleteMultipartUploadRequest
        {
            BucketName = multipart.BucketName,
            Key = multipart.Key,
            UploadId = multipart.UploadId,
            PartETags = parts.Select(p => new PartETag { PartNumber = p.PartNumber, ETag = p.ETag }).ToList()
        };
        var completeResponse = await _s3.CompleteMultipartUploadAsync(completeRequest);
    }
}