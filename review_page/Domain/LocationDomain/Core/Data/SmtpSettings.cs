namespace review_page.Domain.LocationDomain.Core.Data
{
    public record SmtpSettings
    {
        public string Host { get; init; } = string.Empty;
        public string Username { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string SenderName { get; init; } = string.Empty;
        public string SenderEmail { get; init; } = string.Empty;
        public int Port { get; init; } = 587;
        public bool UseSsl { get; init; } = true;
    }
}
