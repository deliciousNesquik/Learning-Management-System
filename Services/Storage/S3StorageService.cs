using Amazon.S3;
using Amazon.S3.Model;
using LMS.Interfaces;
using LMS.DTOs.Storage;
using Microsoft.Extensions.Options;

namespace LMS.Services.Storage;

public class S3StorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3Options _options;
    
    public S3StorageService(IAmazonS3 s3Client, IOptions<S3Options> options)
    {
        _s3Client = s3Client;
        _options = options.Value;
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        var fileKey = $"{fileName}_{Guid.NewGuid()}";
        
        var putRequest = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = fileKey,
            InputStream = fileStream,
            ContentType = contentType
        };

        await _s3Client.PutObjectAsync(putRequest);
        return fileKey; 
    }

    public async Task DeleteFileAsync(string fileKey)
    {
        await _s3Client.DeleteObjectAsync(_options.BucketName, fileKey);
    }

    public string GetPresignedUrl(string fileKey, double durationMinutes = 3)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _options.BucketName,
            Key = fileKey,
            Expires = DateTime.UtcNow.AddMinutes(durationMinutes),
        };

        return _s3Client.GetPreSignedURL(request);
    }
}