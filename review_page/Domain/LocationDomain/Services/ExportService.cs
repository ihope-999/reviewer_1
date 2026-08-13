using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using Document = QuestPDF.Fluent.Document;
using review_page.Domain.LocationDomain.Core.Data;
using review_page.Domain.LocationDomain.Core.Interfaces;

namespace review_page.Domain.LocationDomain.Services
{
    public class ExportService : IExportService
    {
        public byte[] ExportToExcel(IEnumerable<Review> requests)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Review Requests");

            string[] headers = ["ID", "Business", "Place ID", "Recipient", "Email", "Sent", "Created"];
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(1, i + 1).Value = headers[i];
                ws.Cell(1, i + 1).Style.Font.Bold = true;
            }

            int row = 2;
            foreach (var r in requests)
            {
                ws.Cell(row, 1).Value = r.Id;
                ws.Cell(row, 2).Value = r.BusinessName;
                ws.Cell(row, 3).Value = r.PlaceId;
                ws.Cell(row, 4).Value = r.RecipientEmail;
                ws.Cell(row, 5).Value = r.EmailSent ? "Yes" : "No";
                ws.Cell(row, 6).Value = r.CreatedAt.ToString("yyyy-MM-dd HH:mm");
                row++;
            }

            ws.Columns().AdjustToContents();
            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        public byte[] ExportToPdf(IEnumerable<Review> list)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);

                    page.Header().Text("Google Review Requests Report")
                        .FontSize(16).Bold().AlignCenter();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(1); // ID
                            c.RelativeColumn(3); // Business
                            c.RelativeColumn(3); // Recipient
                            c.RelativeColumn(1); // Sent
                            c.RelativeColumn(2); // Date
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background("#2563EB").Text("ID").FontColor("#FFFFFF").Bold();
                            header.Cell().Background("#2563EB").Text("Business").FontColor("#FFFFFF").Bold();
                            header.Cell().Background("#2563EB").Text("Recipient").FontColor("#FFFFFF").Bold();
                            header.Cell().Background("#2563EB").Text("Sent").FontColor("#FFFFFF").Bold();
                            header.Cell().Background("#2563EB").Text("Date").FontColor("#FFFFFF").Bold();
                        });
                        foreach (var r in list)
                        {
                            table.Cell().Text(r.Id.ToString());
                            table.Cell().Text(r.BusinessName);
                            table.Cell().Text($"<{r.RecipientEmail}>");
                            table.Cell().Text(r.EmailSent ? "✓" : "✗");
                            table.Cell().Text(r.CreatedAt.ToString("dd MMM yyyy"));
                        }
                    });

                    page.Footer().AlignCenter()
                        .Text(x => { x.CurrentPageNumber(); x.Span(" / "); x.TotalPages(); });
                });
            }).GeneratePdf();
        }
    }
}
