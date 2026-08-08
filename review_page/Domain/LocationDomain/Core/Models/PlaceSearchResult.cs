namespace review_page.Domain.LocationDomain.Core.Models
{
    public record PlaceSearchResult
    {
        public string PlaceId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ReviewURL { get; set; }
    }
}
