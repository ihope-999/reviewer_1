using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using review_page.Domain.LocationDomain.Core.Data;
using review_page.Domain.LocationDomain.Core.Interfaces;
using review_page.Domain.LocationDomain.Database;

namespace review_page.Domain.Controller
{

    [ApiController]
    [Route("api/review")]
    public class ReviewLinkController : ControllerBase
    {
        private readonly IGooglePlacesService _googlePlacesService;
        private readonly IQrCodeService _qrCodeService;
        private readonly IEmailService _emailService;
        private readonly IExportService _exportService;
        private readonly ReviewDBContext _db;
        private readonly ILogger<ReviewLinkController> _logger;

        public ReviewLinkController(IGooglePlacesService googlePlacesService,
                                    IQrCodeService qrCodeService,
                                    IEmailService emailService,
                                    IExportService exportService,
                                    ReviewDBContext db,
                                    ILogger<ReviewLinkController> logger)
        {
            _googlePlacesService = googlePlacesService;
            _qrCodeService = qrCodeService;
            _emailService = emailService;
            _exportService = exportService;
            _db = db;
            _logger = logger;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateAsync([FromBody] GenerateRequest req)
        {
            try
            {
                _logger.LogInformation("Starting GenerateAsync for business '{BusinessName}'", req.BusinessName);

                var place = await _googlePlacesService.FindPlaceAsync(req.BusinessName);
                if (place == null)
                {
                    _logger.LogWarning("FindPlaceAsync returned null for '{BusinessName}'", req.BusinessName);
                    return NotFound("Business Not Found on Google Maps");
                }
                _logger.LogCritical("Found place: {PlaceId} - {PlaceName}", place.PlaceId, place.Name);

                byte[] qrBytes = Array.Empty<byte>();
                try
                {
                    qrBytes = _qrCodeService.GenerateQrCode(place.ReviewURL);
                    _logger.LogInformation("QR code generated ({Bytes} bytes)", qrBytes?.Length ?? 0);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "QR code generation failed for {PlaceId}", place.PlaceId);
                    return StatusCode(500, new { message = "QR generation failed", detail = ex.Message });
                }

                var qrBase64 = Convert.ToBase64String(qrBytes!);

                var reviewToAdd = new Review
                {
                    BusinessName = req.BusinessName,
                    PlaceId = place.PlaceId,
                    ReviewURL = place.ReviewURL,
                    RecipientEmail = req.RecipientEmail,
                    RecipientName = req.RecipientName,
                    QrCodeB64 = qrBase64,
                    EmailSent = false
                };

                try
                {
                    await _db.ReviewDB.AddAsync(reviewToAdd);
                    _logger.LogCritical("Review added to DbSet for {BusinessName}", reviewToAdd.BusinessName);
                    _logger.LogInformation("Review record prepared for DB insert (Business: {BusinessName})", reviewToAdd.BusinessName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to add Review to DbSet");
                    return StatusCode(500, new { message = "DB add failed", detail = ex.Message });
                }

                if (req.SendEmail)
                {
                    try
                    {
                        await _emailService.SendReviewRequestAsync(req.RecipientEmail, req.RecipientName, place.Name, place.ReviewURL, qrBytes!);
                        reviewToAdd.EmailSent = true;
                        _logger.LogInformation("Email sent to {RecipientEmail}", req.RecipientEmail);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send review email to {RecipientEmail} for {BusinessName}.", req.RecipientEmail, req.BusinessName);
                    }
                }

                try
                {
                    await _db.SaveChangesAsync();
                    _logger.LogInformation("Saved review record {Id} to database.", reviewToAdd.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to save changes to database for review record.");
                    return StatusCode(500, new { message = "DB save failed", detail = ex.Message });
                }

                return Ok(new { reviewToAdd.Id, PlaceName = place.Name, reviewToAdd.ReviewURL, reviewToAdd.QrCodeB64, reviewToAdd.EmailSent });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate review request for {BusinessName}.", req.BusinessName);
                return StatusCode(500, new { message = "An error occurred while generating the review request.", detail = ex.Message });
            }
        }

        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportExcelAsync()
        {
            var data = await _db.ReviewDB.ToListAsync();
            var bytes = _exportService.ExportToExcel(data);
            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"review-requests-{DateTime.Today:yyyyMMdd}.xlsx");
        }

        [HttpGet("export/pdf")]
        public async Task<IActionResult> ExportPDFAsync()
        {
            var data = await _db.ReviewDB.ToListAsync();
            var bytes = _exportService.ExportToPdf(data);
            return File(bytes, "application/pdf",
                $"review-requests-{DateTime.Today:yyyyMMdd}.pdf");
        }
        [HttpGet("review-link")]
        public async Task<IActionResult> GetReviewLinkAsync([FromQuery] string businessName, [FromQuery] string recipientEmail)
        {
            businessName = businessName?.Trim() ?? string.Empty;
            recipientEmail = recipientEmail?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(businessName) || string.IsNullOrWhiteSpace(recipientEmail))
            {
                return BadRequest("businessName and recipientEmail are required.");
            }

            var lowerBusinessName = businessName.ToLower();
            var lowerRecipientEmail = recipientEmail.ToLower();

            var review = await _db.ReviewDB.FirstOrDefaultAsync(r =>
                r.BusinessName.ToLower() == lowerBusinessName &&
                r.RecipientEmail.ToLower() == lowerRecipientEmail);

            if (review == null)
            {
                return NotFound("Review link not found for the given business and email.");
            }

            return Redirect(review.ReviewURL);
        }
    }
}

