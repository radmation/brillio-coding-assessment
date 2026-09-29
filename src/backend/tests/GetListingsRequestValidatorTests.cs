using Backend.Application.Listings;

namespace Backend.Tests;

public class GetListingsRequestValidatorTests
{
    [Fact]
    public void Min_price_greater_than_max_price_is_invalid()
    {
        var errors = GetListingsRequestValidator.Validate(new GetListingsRequest
        {
            MinPrice = 600_000m,
            MaxPrice = 400_000m
        });

        Assert.NotNull(errors);
        Assert.Contains("Min price cannot be greater than max price.", errors["minPrice"]);
    }

    [Fact]
    public void Equal_min_and_max_price_is_valid()
    {
        var errors = GetListingsRequestValidator.Validate(new GetListingsRequest
        {
            MinPrice = 400_000m,
            MaxPrice = 400_000m
        });

        Assert.Null(errors);
    }

    [Fact]
    public void Only_one_price_bound_is_valid()
    {
        Assert.Null(GetListingsRequestValidator.Validate(new GetListingsRequest { MinPrice = 600_000m }));
        Assert.Null(GetListingsRequestValidator.Validate(new GetListingsRequest { MaxPrice = 400_000m }));
    }
}
