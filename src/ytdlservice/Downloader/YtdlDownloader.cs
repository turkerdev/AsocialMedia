using System.Diagnostics;
using System.Text;
using Amazon.S3;
using Amazon.S3.Model;

namespace ytdlservice.Downloader;

internal class YtdlDownloader
{
    private static readonly AmazonS3Client s3 = new(
        ConfigManager.Env.BUCKET_KEY,
        ConfigManager.Env.BUCKET_SECRET,
        new AmazonS3Config
        {
            ServiceURL = ConfigManager.Env.BUCKET_URL,
            ForcePathStyle = true,
        });

    public static async Task Download(string Key, string Url, string Format, string StartTime, string EndTime)
    {
        using var process = new Process();
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.FileName = "ytdlp";
        process.StartInfo.ArgumentList.Add(Url);
        process.StartInfo.ArgumentList.Add("-f");
        process.StartInfo.ArgumentList.Add(Format);
        process.StartInfo.ArgumentList.Add("--download-sections");
        process.StartInfo.ArgumentList.Add($"*{StartTime}-{EndTime}");
        process.StartInfo.ArgumentList.Add("--remux-video");
        process.StartInfo.ArgumentList.Add("mp4");
        process.StartInfo.ArgumentList.Add("-o");
        process.StartInfo.ArgumentList.Add("-");

        process.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);

        process.Start();
        process.BeginErrorReadLine();

        var multipart = await s3.InitiateMultipartUploadAsync(new InitiateMultipartUploadRequest
        {
            BucketName = "ytdl",
            Key = Key
        });

        var partNumber = 1;
        var parts = new List<UploadPartResponse>();
        using var stream = process.StandardOutput.BaseStream;
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
                PartNumber = partNumber,
                PartSize = bufferPosition,
                InputStream = new MemoryStream(buffer, 0, bufferPosition)
            };
            var uploadPartResponse = await s3.UploadPartAsync(uploadPartRequest);
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
        var completeResponse = await s3.CompleteMultipartUploadAsync(completeRequest);

        process.WaitForExit();
    }
}
