using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using review_page.Domain.LocationDomain.Core.Data;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IHttpClientFactory _httpClientFactory;


    [BindProperty]
    public GenerateRequest Input { get; set; } = new();

    public GenerateResponse? GeneratedReview { get; set; }

    public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri($"{Request.Scheme}://{Request.Host}");
        var url = "/api/review/generate";
        _logger.LogInformation("Calling ReviewApi at {BaseAddress}{Url}", client.BaseAddress, url);

        try
        {
            var generateResult = await client.PostAsJsonAsync(url, Input);
            _logger.LogInformation("ReviewApi response status: {StatusCode}", generateResult.StatusCode);

            if (!generateResult.IsSuccessStatusCode)
            {
                var error = await generateResult.Content.ReadAsStringAsync();
                _logger.LogWarning("Generate request failed: {Status} {Error}", generateResult.StatusCode, error);
                ModelState.AddModelError(string.Empty, "Failed to generate review request. " + error);
                return Page();
            }

            GeneratedReview = await generateResult.Content.ReadFromJsonAsync<GenerateResponse>();
            if (GeneratedReview == null)
            {
                ModelState.AddModelError(string.Empty, "Failed to read response from review API.");
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling ReviewApi at {BaseAddress}{Url}", client.BaseAddress, url);
            return BadRequest($"Error calling review API: {ex.Message}");
        }
    }
}

public record GenerateResponse(int Id, string PlaceName, string ReviewURL, string QrCodeB64, bool EmailSent);
