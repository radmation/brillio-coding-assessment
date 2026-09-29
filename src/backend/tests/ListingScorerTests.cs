using Backend.Application.Listings;

namespace Backend.Tests;

public class ListingScorerTests
{
    private static readonly DateTime UtcNow = new(2026, 9, 29, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Exact_price_match_on_list_date_scores_one()
    {
        var listing = ListingFactory.Create("A1", 400_000m, UtcNow);

        var score = ListingScorer.Score(listing, 400_000m, UtcNow);

        Assert.Equal(1m, score);
    }

    [Fact]
    public void Exact_price_can_rank_below_a_newer_near_miss()
    {
        var exactOlder = ListingFactory.Create("B9", 399_500m, new DateTime(2026, 8, 25));
        var nearNewer = ListingFactory.Create("A3", 399_000m, new DateTime(2026, 9, 1));

        var ranked = ListingScorer.Rank([exactOlder, nearNewer], 399_500m, UtcNow);

        Assert.Equal(["A3", "B9"], ranked.Select(l => l.Id));
        Assert.True(ranked[0].Score > ranked[1].Score);
    }

    [Fact]
    public void Tied_scores_break_on_listed_date_then_id()
    {
        var listed = new DateTime(2026, 9, 1);
        var first = ListingFactory.Create("B1", 400_000m, listed);
        var second = ListingFactory.Create("A1", 400_000m, listed);

        var ranked = ListingScorer.Rank([first, second], 400_000m, UtcNow);

        Assert.Equal(["A1", "B1"], ranked.Select(l => l.Id));
        Assert.Equal(ranked[0].Score, ranked[1].Score);
    }

    [Fact]
    public void Future_listed_date_does_not_reduce_recency()
    {
        var listing = ListingFactory.Create("A1", 400_000m, UtcNow.AddDays(3));

        var score = ListingScorer.Score(listing, 400_000m, UtcNow);

        Assert.Equal(1m, score);
    }
}
