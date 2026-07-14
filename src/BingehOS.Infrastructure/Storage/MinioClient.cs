using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace BingehOS.Infrastructure.Storage;

public sealed class MinioClient : IAsyncDisposable
{
    private readonly IMinioClient _client;
    private readonly ILogger<MinioClient> _logger;

    public MinioClient(IConfiguration configuration, ILogger<MinioClient> logger)
    {
        _logger = logger;
        var endpoint = configuration["Minio:Endpoint"] ?? "localhost:9000";
        var accessKey = configuration["Minio:AccessKey"] ?? "minioadmin";
        var secretKey = configuration["Minio:SecretKey"] ?? "minioadmin";
        var secure = bool.Parse(configuration["Minio:Secure"] ?? "false");

        _client = new Minio.MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
            .WithSSL(secure)
            .Build();
    }

    public async Task<bool> BucketExistsAsync(string bucketName, CancellationToken ct = default)
    {
        try
        {
            var args = new BucketExistsArgs().WithBucket(bucketName);
            var result = await _client.BucketExistsAsync(args, ct);
            return result;
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check bucket {Bucket}", bucketName);
            throw;
        }
    }

    public async Task CreateBucketAsync(string bucketName, CancellationToken ct = default)
    {
        try
        {
            var exists = await BucketExistsAsync(bucketName, ct);
            if (exists)
            {
                _logger.LogInformation("Bucket {Bucket} already exists", bucketName);
                return;
            }

            var args = new MakeBucketArgs().WithBucket(bucketName);
            await _client.MakeBucketAsync(args, ct);
            _logger.LogInformation("Created bucket {Bucket}", bucketName);
        }
        catch (ArgumentException ex) when (ex.ParamName == "response" &&
            ex.Message.StartsWith("Bucket already owned by you", StringComparison.Ordinal))
        {
            _logger.LogInformation("Bucket {Bucket} already exists", bucketName);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create bucket {Bucket}", bucketName);
            throw;
        }
    }

    public ValueTask DisposeAsync()
    {
        _client.Dispose();
        return ValueTask.CompletedTask;
    }
}
