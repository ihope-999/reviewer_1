using Microsoft.AspNetCore.Mvc.ViewEngines;
using review_page.Domain.LocationDomain.Core.Data;

namespace review_page.Domain.LocationDomain.Core.Interfaces
{
    public interface IExportService
    {
        public byte[] ExportToExcel(IEnumerable<Review> requests);
        public byte[] ExportToPdf(IEnumerable<Review> list);


    }
}
