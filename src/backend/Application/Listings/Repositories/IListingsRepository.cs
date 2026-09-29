namespace Backend.Application.Listings;

public interface IListingsRepository
{
    Task<IReadOnlyList<Listing>> GetListingsAsync(
        GetListingsRequest getListingsRequest,
        CancellationToken ct = default);
}
