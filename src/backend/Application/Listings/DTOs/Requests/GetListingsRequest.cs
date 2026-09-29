using Microsoft.AspNetCore.Mvc;

namespace Backend.Application.Listings;

public record GetListingsRequest
{
    [FromQuery(Name = "id")] 
    public string? Id { get; init; }

    [FromQuery(Name = "source")] 
    public string? Source { get; init; }

    [FromQuery(Name = "search")] 
    public string? SearchTerm { get; init; }

    [FromQuery(Name = "city")] 
    public string? City { get; init; }

    [FromQuery(Name = "state")] 
    public string? State { get; init; }

    [FromQuery(Name = "status")] 
    public string? Status { get; init; }

    [FromQuery(Name = "minPrice")] 
    public decimal? MinPrice { get; init; }

    [FromQuery(Name = "maxPrice")] 
    public decimal? MaxPrice { get; init; }

    [FromQuery(Name = "minBeds")] 
    public int? MinBedrooms { get; init; }

    [FromQuery(Name = "page")] 
    public int? Page { get; init; }

    [FromQuery(Name = "pageSize")] 
    public int? PageSize { get; init; }

    [FromQuery(Name = "targetBudget")]
    public decimal? TargetBudget { get; init; }
}