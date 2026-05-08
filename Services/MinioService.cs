using Minio;
using Minio.DataModel.Args;

namespace AecTesteTecnico.Services;

public class MinioService
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucket;
    private readonly string _endpoint;

    public MinioService(IMinioClient minioClient, IConfiguration config)
    {
        _minioClient = minioClient;
        _endpoint = config["Minio:Endpoint"];
        _bucket = config["Minio:Bucket"];
    }

    private async Task EnsureBucketExistsAsync()
    {
        var found = await _minioClient.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_bucket));

        if (!found)
        {
            await _minioClient.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(_bucket));
        }
    }

    public async Task<string> UploadAsync(
        IFormFile file,
        string folder,
        string name,
        string id,
        int imageIndex
    )
    {
        if (file == null || file.Length == 0)
            throw new Exception("Arquivo inválido");

        await EnsureBucketExistsAsync();

        var extension = Path.GetExtension(file.FileName);

        var safeName = new string(
            name.ToLower()
                .Where(char.IsLetterOrDigit)
                .ToArray()
        );

        var objectName = $"{folder}/{safeName}-{id}-img{imageIndex}{extension}";

        using var stream = file.OpenReadStream();

        await _minioClient.PutObjectAsync(
            new PutObjectArgs()
                .WithBucket(_bucket)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(file.Length)
                .WithContentType(file.ContentType)
        );

        return objectName;
    }
    public async Task DeleteAsync(string objectName)
    {
        if (string.IsNullOrEmpty(objectName))
            return;

        await _minioClient.RemoveObjectAsync(
            new RemoveObjectArgs()
                .WithBucket(_bucket)
                .WithObject(objectName)
        );
    }
    public string GetFileUrl(string objectName)
    {
        return $"http://{_endpoint}/{_bucket}/{objectName}";
    }

    public string GetImageOrDefault(string? objectName)
    {
        return string.IsNullOrEmpty(objectName)
            ? "/images/default-user.png"
            : GetFileUrl(objectName);
    }

    private string ExtractObjectName(string fileUrl)
    {
        var uri = new Uri(fileUrl);
        return uri.AbsolutePath.Replace($"/{_bucket}/", "");
    }

    public async Task<string> GetPresignedUrlAsync(string objectName, int expirySeconds = 3600)
    {
        return await _minioClient.PresignedGetObjectAsync(
            new PresignedGetObjectArgs()
                .WithBucket(_bucket)
                .WithObject(objectName)
                .WithExpiry(expirySeconds)
        );
    }
}