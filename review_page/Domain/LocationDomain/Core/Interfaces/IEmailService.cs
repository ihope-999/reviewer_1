namespace review_page.Domain.LocationDomain.Core.Interfaces
{
    public interface IEmailService
    {
        public Task SendReviewRequestAsync(
                string toEmail, string toName,
                string businessName, string reviewUrl,
                byte[] qrPngBytes);
    }
}

