namespace review_page.Domain.LocationDomain.Services
{ 

    using Microsoft.Extensions.Options;
    using MimeKit;
    using review_page.Domain.LocationDomain.Core.Data;
    using review_page.Domain.LocationDomain.Core.Interfaces;

    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtp;

        public EmailService(IOptions<SmtpSettings> options) => _smtp = options.Value;

        public async Task SendReviewRequestAsync(
            string toEmail, string toName,
            string businessName, string reviewUrl,
            byte[] qrPngBytes)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtp.SenderName, _smtp.SenderEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = $"Review Request — {businessName}";

            var builder = new BodyBuilder
            {
                HtmlBody = $"""
                <h2>We'd love your review!</h2>
                <p>Hi {toName}, please click the link or scan the QR code below to leave a review for <strong>{businessName}</strong>.</p>
                <p><a href="{reviewUrl}">{reviewUrl}</a></p>
                <img src="cid:qrcode" alt="QR Code" />
                """
            };

            var qrAttachment = builder.LinkedResources.Add("qrcode.png", qrPngBytes);
            qrAttachment.ContentId = "qrcode";
            qrAttachment.ContentType.MediaType = "image";
            qrAttachment.ContentType.MediaSubtype = "png";

            message.Body = builder.ToMessageBody();

            using var client = new MailKit.Net.Smtp.SmtpClient();
            var secureSocketOptions = _smtp.UseSsl
                ? MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable
                : MailKit.Security.SecureSocketOptions.None;

            await client.ConnectAsync(_smtp.Host, _smtp.Port, secureSocketOptions);
            await client.AuthenticateAsync(_smtp.Username, _smtp.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
