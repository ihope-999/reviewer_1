using System.ComponentModel.DataAnnotations;

namespace review_page.Domain.LocationDomain.Core.Data
{
    public class GenerateRequest
    {
        [Required(ErrorMessage = "Business name is required.")]
        public string BusinessName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Recipient email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string RecipientEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Recipient name is required.")]
        public string RecipientName { get; set; } = string.Empty;

        public bool SendEmail { get; set; }
    }
}
