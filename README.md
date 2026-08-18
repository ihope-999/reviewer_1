# Review Link Generator

## What It Does

- Enter a business name and the customer's name and email.
- Look up the business using the Google Places API.
- Generate a direct link to the business's Google review page.
- Generate a QR code pointing to the review link.
- Optionally email the review link and QR code to the customer.
- Store each request in a database.
- Export request records to `.xlsx` or `.pdf`.

## Tech Stack

- **ASP.NET Core** — Web application framework
- **Entity Framework Core** — Database access
- **MailKit / MimeKit** — Email and SMTP
- **QRCoder** — QR code generation
- **ClosedXML** — Excel (.xlsx) generation
- **QuestPDF** — PDF generation
- **Google Places API** — Business lookup and Place information

## Third-Party Licenses & Terms

This project uses third-party libraries and services that are subject to their respective licenses and terms.

| Technology | License / Terms |
|---|---|
| ASP.NET Core | MIT |
| Entity Framework Core | MIT |
| MailKit | MIT |
| MimeKit | MIT |
| QRCoder | MIT |
| ClosedXML | MIT |
| QuestPDF | QuestPDF Community License |
| Google Places API | Google Maps Platform Terms and API policies |

## Notes

- Third-party libraries remain subject to their respective licenses.
- Google Places API usage is subject to Google's current terms and policies.
- No third-party trademarks or services are claimed as part of this project.
