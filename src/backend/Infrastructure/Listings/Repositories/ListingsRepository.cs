using System.Text.Json;
using Backend.Application.Listings;

namespace Backend.Infrastructure.Listings;

public class ListingsRepository : IListingsRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly string _filePath;

    public ListingsRepository(IWebHostEnvironment env)
    {
        _filePath = Path.Combine(
            env.ContentRootPath,
            "Infrastructure",
            "Listings",
            "Data",
            "sample_listings.json");
    }

    public async Task<IReadOnlyList<Listing>> GetListingsAsync(
        GetListingsRequest getListingsRequest,
        CancellationToken ct = default)
    {
        await using var stream = File.OpenRead(_filePath);
        var listings = await JsonSerializer.DeserializeAsync<List<Listing>>(stream, JsonOptions, ct)
                       ?? [];

        IEnumerable<Listing> query = listings;

        if (!string.IsNullOrWhiteSpace(getListingsRequest.Id))
            query = query.Where(l => l.Id.Equals(getListingsRequest.Id, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(getListingsRequest.Source))
            query = query.Where(l => l.Source.Equals(getListingsRequest.Source, StringComparison.OrdinalIgnoreCase));

        // City is exact match. Could swap to contains if desired for partial matches.
        if (!string.IsNullOrWhiteSpace(getListingsRequest.City))
            query = query.Where(l => l.City.Equals(getListingsRequest.City, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(getListingsRequest.State))
            query = query.Where(l => l.State.Equals(getListingsRequest.State, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(getListingsRequest.Status))
            query = query.Where(l => l.Status.Equals(getListingsRequest.Status, StringComparison.OrdinalIgnoreCase));

        if (getListingsRequest.MinPrice.HasValue)
            query = query.Where(l => l.Price >= getListingsRequest.MinPrice.Value);

        if (getListingsRequest.MaxPrice.HasValue)
            query = query.Where(l => l.Price <= getListingsRequest.MaxPrice.Value);

        if (getListingsRequest.MinBedrooms.HasValue)
            query = query.Where(l => l.Bedrooms >= getListingsRequest.MinBedrooms.Value);

        if (!string.IsNullOrWhiteSpace(getListingsRequest.SearchTerm))
            query = query.Where(l =>
                l.Description.Contains(getListingsRequest.SearchTerm, StringComparison.OrdinalIgnoreCase));

        return query.ToList();
    }
}
