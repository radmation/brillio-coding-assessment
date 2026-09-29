using Backend.Application.Listings;

namespace Backend.Tests;

public class ListingServiceTests
{
    private readonly FakeListingsRepository _repository = new();
    private readonly ListingService _service;

    public ListingServiceTests()
    {
        _service = new ListingService(_repository);
    }

    [Fact]
    public async Task No_matches_returns_empty_page()
    {
        _repository.Items = [];

        var result = await _service.GetListingsAsync(new GetListingsRequest { Page = 1, PageSize = 5 });

        Assert.Empty(result.Listings);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(5, result.PageSize);
    }

    [Fact]
    public async Task Pagination_returns_the_requested_slice()
    {
        _repository.Items = Enumerable.Range(1, 5)
            .Select(i => ListingFactory.Create($"A{i}", 400_000m, new DateTime(2026, 9, i)))
            .ToList();

        var result = await _service.GetListingsAsync(new GetListingsRequest { Page = 2, PageSize = 2 });

        Assert.Equal(5, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(["A3", "A4"], result.Listings.Select(l => l.Id));
    }

    [Fact]
    public async Task Last_page_can_be_a_partial_page()
    {
        _repository.Items = Enumerable.Range(1, 5)
            .Select(i => ListingFactory.Create($"A{i}", 400_000m, new DateTime(2026, 9, i)))
            .ToList();

        var result = await _service.GetListingsAsync(new GetListingsRequest { Page = 3, PageSize = 2 });

        Assert.Single(result.Listings);
        Assert.Equal("A5", result.Listings[0].Id);
        Assert.Equal(5, result.TotalCount);
    }

    [Fact]
    public async Task Page_past_the_end_returns_no_items()
    {
        _repository.Items = [ListingFactory.Create("A1", 400_000m, new DateTime(2026, 9, 1))];

        var result = await _service.GetListingsAsync(new GetListingsRequest { Page = 4, PageSize = 5 });

        Assert.Empty(result.Listings);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(4, result.Page);
    }

    [Fact]
    public async Task Missing_or_invalid_page_values_default_to_page_one_size_five()
    {
        _repository.Items = Enumerable.Range(1, 6)
            .Select(i => ListingFactory.Create($"A{i}", 400_000m, new DateTime(2026, 9, i)))
            .ToList();

        var result = await _service.GetListingsAsync(new GetListingsRequest { Page = 0, PageSize = -1 });

        Assert.Equal(1, result.Page);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(5, result.Listings.Count);
        Assert.Equal("A1", result.Listings[0].Id);
    }

    [Fact]
    public async Task Target_budget_sorts_before_pagination()
    {
        _repository.Items =
        [
            ListingFactory.Create("Far", 800_000m, new DateTime(2026, 9, 1)),
            ListingFactory.Create("Near", 410_000m, new DateTime(2026, 9, 1))
        ];

        var result = await _service.GetListingsAsync(new GetListingsRequest
        {
            TargetBudget = 400_000m,
            Page = 1,
            PageSize = 1
        });

        Assert.Equal("Near", result.Listings[0].Id);
        Assert.NotNull(result.Listings[0].Score);
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task Zero_target_budget_does_not_reorder()
    {
        _repository.Items =
        [
            ListingFactory.Create("Far", 800_000m, new DateTime(2026, 9, 1)),
            ListingFactory.Create("Near", 410_000m, new DateTime(2026, 9, 1))
        ];

        var result = await _service.GetListingsAsync(new GetListingsRequest
        {
            TargetBudget = 0,
            Page = 1,
            PageSize = 5
        });

        Assert.Equal(["Far", "Near"], result.Listings.Select(l => l.Id));
        Assert.All(result.Listings, listing => Assert.Null(listing.Score));
    }
}
