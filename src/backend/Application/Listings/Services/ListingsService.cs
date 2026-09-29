namespace Backend.Application.Listings;

public class ListingService : IListingsService
{
    private readonly IListingsRepository _listingsRepository;

    public ListingService(IListingsRepository listingsRepository)
    {
        _listingsRepository = listingsRepository;
    }

    public async Task<PagedResponse> GetListingsAsync(GetListingsRequest filter, CancellationToken ct = default)
    {
        var listings = await _listingsRepository.GetListingsAsync(filter, ct);

        if (filter.TargetBudget is > 0)
            listings = ListingScorer.Rank(listings, filter.TargetBudget.Value);

        var page = filter.Page is null or < 1 ? 1 : filter.Page.Value;
        // TODO: Put page size in a constant.
        var pageSize = filter.PageSize is null or < 1 ? 5 : filter.PageSize.Value;
        var items = listings.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new PagedResponse(items, listings.Count, page, pageSize);
    }
}
