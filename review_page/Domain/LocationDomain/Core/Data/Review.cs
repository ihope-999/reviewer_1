namespace review_page.Domain.LocationDomain.Core.Data
{
    public class Review
    {
        public Review() { }
        public int Id { get; set; }
        public string BusinessName { get; set; }
        public string PlaceId { get; set; }
        public string RecipientName { get; set; }
        public string ReviewURL { get; set; }
        public string RecipientEmail { get; set; }
        public bool EmailSent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? QrCodeB64 { get; set; }

    }
}
