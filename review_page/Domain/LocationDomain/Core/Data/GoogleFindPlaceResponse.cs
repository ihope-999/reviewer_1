using System.Text.Json.Serialization;

namespace review_page.Domain.LocationDomain.Core.Data
{
    public record GooglePlaceCandidate
    {
        [property: JsonPropertyName("place_id")]
        public string PlaceId;

        [property: JsonPropertyName("name")]
        public string Name;

        [property: JsonPropertyName("formatted_address")]
        public string FormattedAddress;

    }


    public record GoogleFindPlaceResponse
    {
        [property: JsonPropertyName("candidates")]
        public List<GooglePlaceCandidate>? Candidates;
        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }
}
