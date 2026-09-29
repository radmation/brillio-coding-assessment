using Backend.Application.Listings;

namespace Backend.Tests;

internal static class ListingFactory
{
    public static Listing Create(
        string id,
        decimal price,
        DateTime listedDate,
        string city = "Fairfax")
    {
        return new Listing(
            id,
            "MLS_A",
            "1 Main St",
            city,
            "VA",
            "22030",
            price,
            2,
            1,
            900,
            38.8,
            -77.3,
            listedDate,
            "active",
            "Test listing");
    }
}

internal sealed class FakeListingsRepository : IListingsRepository
{
    public IReadOnlyList<Listing> Items { get; set; } = [];

    public Task<IReadOnlyList<Listing>> GetListingsAsync(
        GetListingsRequest getListingsRequest,
        CancellationToken ct = default)
    {
        return Task.FromResult(Items);
    }
}
