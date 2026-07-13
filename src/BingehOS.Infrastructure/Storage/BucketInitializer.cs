using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BingehOS.Infrastructure.Storage;

public sealed class BucketInitializer : IHostedService
{
    private readonly MinioClient _minioClient;
    private readonly ILogger<BucketInitializer> _logger;

    public BucketInitializer(MinioClient minioClient, ILogger<BucketInitializer> logger)
    {
        _minioClient = minioClient;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var buckets = new[] { "facility-documents", "work-order-attachments" };

        foreach (var bucket in buckets)
        {
            try
            {
                await _minioClient.CreateBucketAsync(bucket, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bucket initialization failed for {Bucket}", bucket);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _minioClient.DisposeAsync();
        return Task.CompletedTask;
    }
}
