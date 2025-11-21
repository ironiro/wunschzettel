using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using wunschzettel.Services;

namespace wunschzettel.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly WishlistStore _store;

    public IndexModel(ILogger<IndexModel> logger, WishlistStore store)
    {
        _logger = logger;
        _store = store;
    }

    [BindProperty]
    public string? CreateInput { get; set; }

    [BindProperty]
    public string? OpenInput { get; set; }

    [TempData]
    public string? Message { get; set; }

    public void OnGet()
    {

    }

    public IActionResult OnPostCreate()
    {
        _logger.LogInformation("Create button clicked. Value: {value}", CreateInput);
        var wishlist = _store.Create(CreateInput);
        // redirect to wishlist view by ID
        return RedirectToPage("/Wishlist", new { id = wishlist.Id });
    }

    public IActionResult OnPostOpen()
    {
        _logger.LogInformation("Open button clicked. Value: {value}", OpenInput);
        if (string.IsNullOrWhiteSpace(OpenInput))
        {
            ModelState.AddModelError(string.Empty, "Bitte eine ID oder einen Namen angeben.");
            return Page();
        }

        var id = OpenInput.Trim();
        // allow opening by 4-digit numeric id only for now
        if (_store.TryGet(id, out var wishlist))
        {
            return RedirectToPage("/Wishlist", new { id = id });
        }

        ModelState.AddModelError(string.Empty, "Wunschzettel nicht gefunden. Bitte prüfen Sie die ID.");
        return Page();
    }
}
