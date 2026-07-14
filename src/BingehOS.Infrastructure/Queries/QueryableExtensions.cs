namespace BingehOS.Infrastructure.Queries;

public static class QueryableExtensions
{
    private const int DefaultPageSize = 20;

    public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int skip, int take)
    {
        var normalizedSkip = Math.Max(skip, 0);
        var normalizedTake = take <= 0 ? DefaultPageSize : take;

        return query.Skip(normalizedSkip).Take(normalizedTake);
    }
}
