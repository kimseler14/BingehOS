using Microsoft.Extensions.Hosting;

namespace BingehOS.Infrastructure.Storage;

public sealed class BucketInitializer : IHostedService
{
    private readonly MinioClient _minioClient;

    public BucketInitializer(MinioClient minioClient) => _minioClient = minioClient;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var buckets = new[] { "facility-documents", "work-order-attachments" };

        foreach (var bucket in buckets)
            await _minioClient.CreateBucketAsync(bucket, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken) =>
        await _minioClient.DisposeAsync();
}
