using Backend.Application.Listings;
using Backend.Endpoints;
using Backend.Infrastructure.Listings;

var builder = WebApplication.CreateBuilder(args);

// =========================================================================
// 1. REGISTER SERVICES INTO DI CONTAINER (builder.Services)
// =========================================================================

// Allow Cross-Origin Requests from Angular Client (http://localhost:4200)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add open api
builder.Services.AddOpenApi();

builder.Services.AddScoped<IListingsService, ListingService>();
builder.Services.AddScoped<IListingsRepository, ListingsRepository>();

// =========================================================================
// 2. BUILD THE APPLICATION
// =========================================================================
var app = builder.Build();

// =========================================================================
// 3. CONFIGURE MIDDLEWARE PIPELINE & ENDPOINTS (app)
// =========================================================================
// Enable OpenAPI JSON in Development mode
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Exposes /openapi/v1.json
}

// Enable CORS
app.UseCors("AllowAngular");

// Map Minimal API Endpoints (from Endpoints/ListingEndpoints.cs)
app.MapListingEndpoints();

app.Run();