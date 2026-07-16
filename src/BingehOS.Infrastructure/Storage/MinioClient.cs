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
        var endpointRaw = configuration["Minio:Endpoint"] ?? "localhost:9000";
        var accessKey = configuration["Minio:AccessKey"] ?? "minioadmin";
        var secretKey = configuration["Minio:SecretKey"] ?? "minioadmin";
        var secure = bool.Parse(configuration["Minio:Secure"] ?? "false");

        var endpoint = endpointRaw;
        if (Uri.TryCreate(endpointRaw, UriKind.Absolute, out var uri))
        {
            endpoint = uri.Host + ":" + uri.Port;
            secure = string.Equals(uri.Scheme, "https", StringComparison.OrdinalIgnoreCase);
        }

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
            return false;
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
        catch (ArgumentException ex)
        {
            ct.ThrowIfCancellationRequested();

            try
            {
                if (await BucketExistsAsync(bucketName, ct))
                {
                    _logger.LogInformation("Bucket {Bucket} already exists", bucketName);
                    return;
                }
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception verificationException)
            {
                _logger.LogWarning(
                    verificationException,
                    "Failed to verify bucket {Bucket} after creation failed",
                    bucketName);
            }

            _logger.LogError(ex, "Failed to create bucket {Bucket}", bucketName);
            throw;
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create bucket {Bucket}", bucketName);
        }
    }

    public ValueTask DisposeAsync()
    {
        _client.Dispose();
        return ValueTask.CompletedTask;
    }
}
