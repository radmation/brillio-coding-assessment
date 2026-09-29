namespace Backend.Application.Listings;

public static class GetListingsRequestValidator
{
    public static IDictionary<string, string[]>? Validate(GetListingsRequest filter)
    {
        if (filter.MinPrice.HasValue && filter.MaxPrice.HasValue && filter.MinPrice.Value > filter.MaxPrice.Value)
        {
            return new Dictionary<string, string[]>
            {
                ["minPrice"] = ["Min price cannot be greater than max price."]
            };
        }

        return null;
    }
}
