namespace Backend.Application.Listings;

/**
Note on scorer - decimal: Slower, but offers extreme precision with zero base-2 rounding errors. 
We could swap to float or int if performance is an issue.

This will produce a final score between 0 and 1.
Final Score = (0.7 * Price Score) + (0.3 * Recency Score)
**/
public static class ListingScorer
{
    public const decimal PriceWeight = 0.7m;
    public const decimal RecencyWeight = 0.3m;

    public static decimal Score(Listing listing, decimal targetBudget) =>
        Score(listing, targetBudget, DateTime.UtcNow);

    public static decimal Score(Listing listing, decimal targetBudget, DateTime utcNow)
    {
        // Price score calculation.
        var priceDelta = Math.Abs(listing.Price - targetBudget) / targetBudget;
        var priceScore = 1m / (1m + priceDelta);
        
        // TODO: Could penalize listings that are over the target budget instead of treating them the same.
        // Break priceScore out into a separate value and add conditions (greater than or less than target budget) that returns a score and a penalty.
        // The squaring doesn't hurt tiny overages but the 5x multiplier for larger overages is a bit harsh.
        // Ie: if (priceDelta <= 0) ? 1m : 1m / (1m + (priceDelta * priceDelta * 5m); 

        // Day score calculation.
        var daysSinceListed = (decimal)Math.Max(0, (utcNow.Date - listing.ListedDate.Date).TotalDays);
        var recencyScore = 1m / (1m + daysSinceListed);

        // TODO: Could switch to a different formula if desired
        // Linear decay: Drops by 1% per day, floors at 0 after 30 days
        // var recencyScore = Math.Max(0m, 1m - (daysSinceListed * 0.01m));

        // Exponential decay: lambda (0.05) controls how fast it drops
        // Day 0 = 1.0 | Day 7 = 0.70 | Day 30 = 0.22 | Day 90 = 0.01
        // var recencyScore = (decimal)Math.Exp(-0.05 * daysSinceListed);

        // TODO: Older listings could be considered more relevant than newer listings, more flexible in pricing ect!

        return PriceWeight * priceScore + RecencyWeight * recencyScore;
    }

    public static IReadOnlyList<Listing> Rank(
        IEnumerable<Listing> listings,
        decimal targetBudget,
        DateTime? utcNow = null)
    {
        var now = utcNow ?? DateTime.UtcNow;
        return listings
            .Select(listing => listing with { Score = Score(listing, targetBudget, now) })
            .OrderByDescending(listing => listing.Score)
            .ThenByDescending(listing => listing.ListedDate)
            .ThenBy(listing => listing.Id, StringComparer.Ordinal)
            .ToList();
    }
}
