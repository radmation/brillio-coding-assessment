using System.Text.Json.Serialization;

namespace Backend.Application.Listings;

public record Listing(
    string Id,
    string Source,
    string Address,
    string City,
    string State,
    string Zip,
    decimal Price,
    int Bedrooms,
    float Bathrooms,
    int Sqft,
    double Latitude,
    double Longitude,
    DateTime ListedDate,
    string Status,
    string Description,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    decimal? Score = null
);

public record PagedResponse(
    IReadOnlyList<Listing> Listings,
    int TotalCount,
    int Page,
    int PageSize
);
