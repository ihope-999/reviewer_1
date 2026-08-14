using review_page.Domain.LocationDomain.Core.Interfaces;
using review_page.Domain.LocationDomain.Core.Models;

namespace review_page.Domain.LocationDomain.Services
{
    public class GooglePlacesService : IGooglePlacesService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _configuration;
        private readonly string apiKey;
        private readonly ILogger<GooglePlacesService> _logger;
        public GooglePlacesService(HttpClient http, IConfiguration configuration, ILogger<GooglePlacesService> logger)
        {
            _logger = logger;
            _http = http;
            _configuration = configuration;
            apiKey = configuration["Google:apiKey"] ?? throw new InvalidOperationException("Google API key is not configured.");
        }
        public async Task<PlaceSearchResult?> FindPlaceAsync(string query)
        {
            var url = $"https://maps.googleapis.com/maps/api/place/textsearch/json" +
              $"?query={Uri.EscapeDataString(query)}" +
              $"&key={apiKey}";

            try
            {
                var response = await _http.GetFromJsonAsync<GoogleFindPlaceResponse>(url);
                _logger.LogInformation("Google API status: {Status}", response?.Status);

                var candidate = response?.Candidates?.FirstOrDefault();
                if (candidate is not null)
                {
                    return new PlaceSearchResult
                    {
                        PlaceId = candidate.PlaceId,
                        Name = candidate.Name,
                        Address = candidate.FormattedAddress,
                        ReviewURL = $"https://search.google.com/local/writereview?placeid={candidate.PlaceId}"
                    };
                }

                _logger.LogInformation("No candidates from findplacefortext for '{Query}', trying textsearch.", query);
                var textSearchUrl = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={Uri.EscapeDataString(query)}&key={apiKey}";
                var textResponse = await _http.GetFromJsonAsync<GoogleTextSearchResponse>(textSearchUrl);
                _logger.LogInformation("Google TextSearch status: {Status}", textResponse?.Status);
                var textResult = textResponse?.Results?.FirstOrDefault();
                if (textResult is null)
                {
                    _logger.LogWarning("Google Places found no results for query '{Query}' (findplace and textsearch).", query);
                    return null;
                }

                return new PlaceSearchResult
                {
                    PlaceId = textResult.PlaceId,
                    Name = textResult.Name,
                    Address = textResult.FormattedAddress ?? textResult.FormattedAddressAlt,
                    ReviewURL = $"https://search.google.com/local/writereview?placeid={textResult.PlaceId}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Google Places API for query '{Query}'", query);
                return null;
            }
        }
    }
}


public record GoogleTextSearchResult
{
    [property: System.Text.Json.Serialization.JsonPropertyName("place_id")]
    public string PlaceId { get; init; }
    [property: System.Text.Json.Serialization.JsonPropertyName("name")]
    public string Name { get; init; }
    [property: System.Text.Json.Serialization.JsonPropertyName("formatted_address")]
    public string? FormattedAddress { get; init; }
    // some textsearch responses may use 'formatted_address' or provide a 'vicinity' field
    [property: System.Text.Json.Serialization.JsonPropertyName("vicinity")]
    public string? FormattedAddressAlt { get; init; }
}

public record GoogleTextSearchResponse
{
    [property: System.Text.Json.Serialization.JsonPropertyName("results")]
    public List<GoogleTextSearchResult>? Results { get; init; }
    [property: System.Text.Json.Serialization.JsonPropertyName("status")]
    public string? Status { get; init; }
}
