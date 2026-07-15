using BingehOS.Infrastructure;
using BingehOS.Modules.Asset.Domain;
using BingehOS.Modules.Inventory.Domain;
using BingehOS.Modules.Maintenance.Domain;
using Microsoft.EntityFrameworkCore;

namespace BingehOS.Modules.Maintenance.Application;

public sealed record AssetMaintenanceInsight(
    Guid AssetId,
    string AssetName,
    int FailureCount,
    double FailureFrequencyPerMonth,
    double? MeanTimeBetweenFailuresDays,
    string Trend,
    bool ElevatedRecentFailureRate,
    string Risk,
    string Rationale);

public sealed record PartReorderInsight(
    Guid PartId,
    string PartNumber,
    string PartName,
    int CurrentStock,
    int SuggestedReorderThreshold,
    double AverageMonthlyIssueQuantity,
    string Rationale);

public sealed class MaintenanceInsightService(AppDbContext db)
{
    private static readonly TimeSpan RecentWindow = TimeSpan.FromDays(90);

    public async Task<IReadOnlyList<AssetMaintenanceInsight>> GetAssetInsightsAsync(
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var workOrders = await db.WorkOrders
            .AsNoTracking()
            .Where(order => order.TenantId == db.CurrentTenantId)
            .Select(order => new
            {
                order.AssetId,
                order.Description,
                order.CreatedAt
            })
            .ToListAsync(cancellationToken);
        var assets = await db.Assets
            .AsNoTracking()
            .Where(asset => asset.TenantId == db.CurrentTenantId)
            .Select(asset => new { asset.Id, asset.Name })
            .ToDictionaryAsync(asset => asset.Id, cancellationToken);

        return assets
            .Select(asset =>
            {
                var failures = workOrders
                    .Where(order => order.AssetId == asset.Key && IsCorrective(order.Description))
                    .Select(order => order.CreatedAt)
                    .OrderBy(date => date)
                    .ToList();
                return CalculateAssetInsight(asset.Key, asset.Value.Name, failures, now);
            })
            .OrderByDescending(insight => RiskRank(insight.Risk))
            .ThenByDescending(insight => insight.FailureFrequencyPerMonth)
            .ToList();
    }

    public async Task<IReadOnlyList<PartReorderInsight>> GetPartInsightsAsync(
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var parts = await db.Parts
            .AsNoTracking()
            .Where(part => part.TenantId == db.CurrentTenantId && part.IsActive)
            .Select(part => new { part.Id, part.PartNumber, part.Name })
            .ToListAsync(cancellationToken);
        var transactions = await db.InventoryTransactions
            .AsNoTracking()
            .Where(transaction => transaction.TenantId == db.CurrentTenantId)
            .Select(transaction => new
            {
                transaction.PartId,
                transaction.Type,
                transaction.Quantity,
                transaction.TransactionDate
            })
            .ToListAsync(cancellationToken);

        return parts
            .Select(part => CalculatePartInsight(
                part.Id,
                part.PartNumber,
                part.Name,
                transactions
                    .Where(transaction => transaction.PartId == part.Id)
                    .Select(transaction => (transaction.Type, transaction.Quantity, transaction.TransactionDate))
                    .ToList(),
                now))
            .Where(insight => insight is not null)
            .Select(insight => insight!)
            .OrderBy(insight => insight.CurrentStock - insight.SuggestedReorderThreshold)
            .ToList();
    }

    public static AssetMaintenanceInsight CalculateAssetInsight(
        Guid assetId,
        string assetName,
        IReadOnlyList<DateTimeOffset> correctiveDates,
        DateTimeOffset now)
    {
        var failureCount = correctiveDates.Count;
        var observedDays = Math.Max(30, (now - (correctiveDates.FirstOrDefault() == default ? now : correctiveDates.First())).TotalDays);
        var frequency = failureCount == 0 ? 0 : failureCount / (observedDays / 30d);
        var intervals = correctiveDates.Zip(correctiveDates.Skip(1), (first, second) => (second - first).TotalDays).ToList();
        double? mtbf = intervals.Count == 0 ? null : intervals.Average();
        var recentStart = now - RecentWindow;
        var previousStart = recentStart - RecentWindow;
        var recentCount = correctiveDates.Count(date => date >= recentStart);
        var previousCount = correctiveDates.Count(date => date >= previousStart && date < recentStart);
        var recentRate = recentCount / 3d;
        var previousRate = previousCount / 3d;
        var elevated = recentCount > 0 && (previousRate == 0 || recentRate > previousRate * 1.25);
        var trend = elevated ? "Elevated" : recentRate < previousRate * 0.75 ? "Improving" : "Stable";
        var risk = elevated ? "High" : frequency >= 1 ? "Medium" : "Low";
        var rationale = failureCount == 0
            ? "Geçmişte sınıflandırılmış düzeltici iş emri bulunamadı."
            : elevated
                ? $"Son 90 günde {recentCount} düzeltici iş emri var; önceki 90 güne göre arıza oranı yükseldi."
                : $"{failureCount} düzeltici iş emri, aylık yaklaşık {frequency:F1} arıza sıklığı ve {FormatDays(mtbf)} MTBF gösteriyor.";

        return new AssetMaintenanceInsight(
            assetId,
            assetName,
            failureCount,
            Math.Round(frequency, 2),
            mtbf is null ? null : Math.Round(mtbf.Value, 1),
            trend,
            elevated,
            risk,
            rationale);
    }

    public static PartReorderInsight? CalculatePartInsight(
        Guid partId,
        string partNumber,
        string partName,
        IReadOnlyList<(TransactionType Type, int Quantity, DateTimeOffset Date)> transactions,
        DateTimeOffset now)
    {
        if (transactions.Count == 0)
            return null;

        var ordered = transactions.OrderBy(transaction => transaction.Date).ToList();
        var currentStock = 0;
        foreach (var transaction in ordered)
        {
            currentStock = transaction.Type switch
            {
                TransactionType.Receiving or TransactionType.Return => checked(currentStock + transaction.Quantity),
                TransactionType.Issue => currentStock - transaction.Quantity,
                _ => currentStock
            };
        }

        var recentIssues = transactions
            .Where(transaction => transaction.Type == TransactionType.Issue && transaction.Date >= now - TimeSpan.FromDays(180))
            .Sum(transaction => transaction.Quantity);
        var averageMonthlyIssue = recentIssues / 6d;
        var threshold = Math.Max(1, (int)Math.Ceiling(averageMonthlyIssue * 2));
        if (currentStock > threshold || recentIssues == 0)
            return null;

        return new PartReorderInsight(
            partId,
            partNumber,
            partName,
            currentStock,
            threshold,
            Math.Round(averageMonthlyIssue, 1),
            $"Mevcut stok {currentStock}; son 6 ayda aylık ortalama {averageMonthlyIssue:F1} kullanım ve önerilen eşik {threshold}. Yeniden sipariş planlanmalı.");
    }

    public static bool IsCorrective(string description)
    {
        var normalized = description.ToLowerInvariant();
        return normalized.Contains("arıza")
            || normalized.Contains("ariza")
            || normalized.Contains("corrective")
            || normalized.Contains("failure")
            || normalized.Contains("onarım")
            || normalized.Contains("onarim");
    }

    private static int RiskRank(string risk) => risk switch
    {
        "High" => 3,
        "Medium" => 2,
        _ => 1
    };

    private static string FormatDays(double? days) => days is null ? "hesaplanamaz" : $"{days.Value:F0} gün";
}
