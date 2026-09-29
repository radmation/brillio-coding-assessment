using Backend.Application.Listings;

namespace Backend.Endpoints;

public static class ListingEndpoints
{
    public static void MapListingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/listings").WithTags("MLS Listings");

        group.MapGet("/", GetListings);
    }

    private static async Task<IResult> GetListings([AsParameters] GetListingsRequest filter, IListingsService listingService, CancellationToken ct)
    {
        var errors = GetListingsRequestValidator.Validate(filter);
        if (errors is not null)
            return Results.ValidationProblem(errors);

        var response = await listingService.GetListingsAsync(filter, ct);
        
        return Results.Ok(response);
    }
}