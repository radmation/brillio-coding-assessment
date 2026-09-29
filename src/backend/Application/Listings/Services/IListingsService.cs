namespace Backend.Application.Listings;

public interface IListingsService
{
   Task<PagedResponse> GetListingsAsync(GetListingsRequest filter, CancellationToken ct = default);
}