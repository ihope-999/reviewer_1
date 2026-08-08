using review_page.Domain.LocationDomain.Core.Models;

namespace review_page.Domain.LocationDomain.Core.Interfaces
{
    public interface IGooglePlacesService
    {
        Task<PlaceSearchResult?> FindPlaceAsync(string query);

    }
}
