using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using wunschzettel.Models;
using wunschzettel.Services;

namespace wunschzettel.Pages;

public class WishlistModel : PageModel
{
    private readonly WishlistStore _store;

    public WishlistModel(WishlistStore store)
    {
        _store = store;
    }

    [BindProperty(SupportsGet = true)]
    public string? Id { get; set; }

    public Wishlist? Wishlist { get; set; }

    [BindProperty]
    public Gift? NewGift { get; set; }

    public IActionResult OnPostAddGift()
    {
        if (string.IsNullOrWhiteSpace(Id) || NewGift == null || string.IsNullOrWhiteSpace(NewGift.Name))
        {
            ModelState.AddModelError(string.Empty, "Bitte einen Namen für das Geschenk angeben.");
            return OnGet() is IActionResult r ? r : Page();
        }

        // normalize link (trim) to avoid accidental leading/trailing spaces
        if (!string.IsNullOrWhiteSpace(NewGift.Link)) NewGift.Link = NewGift.Link.Trim();

        var success = _store.AddGift(Id.Trim(), NewGift);
        if (!success) return NotFound();
        return RedirectToPage("/Wishlist", new { id = Id });
    }

    public IActionResult OnPostToggleBought(string giftId)
    {
        if (string.IsNullOrWhiteSpace(Id) || string.IsNullOrWhiteSpace(giftId)) return BadRequest();
        var success = _store.ToggleGiftBought(Id.Trim(), giftId);
        if (!success) return NotFound();
        return RedirectToPage("/Wishlist", new { id = Id });
    }

    public IActionResult OnPostMarkBought(string giftId, string? buyer)
    {
        if (string.IsNullOrWhiteSpace(Id) || string.IsNullOrWhiteSpace(giftId)) return BadRequest();

        // Require buyer name
        if (string.IsNullOrWhiteSpace(buyer))
        {
            // Put values in ViewData so the page can reopen the modal and show an error
            ViewData["OpenMarkModal"] = giftId;
            ViewData["MarkBuyerError"] = "Bitte geben Sie den Namen ein.";
            return OnGet();
        }

        var success = _store.SetGiftBought(Id.Trim(), giftId, buyer);
        if (!success) return NotFound();
        return RedirectToPage("/Wishlist", new { id = Id });
    }

    public IActionResult OnPostUnsetBought(string giftId)
    {
        if (string.IsNullOrWhiteSpace(Id) || string.IsNullOrWhiteSpace(giftId)) return BadRequest();
        var success = _store.UnsetGiftBought(Id.Trim(), giftId);
        if (!success) return NotFound();
        return RedirectToPage("/Wishlist", new { id = Id });
    }

    public IActionResult OnPostRemoveGift(string giftId)
    {
        if (string.IsNullOrWhiteSpace(Id) || string.IsNullOrWhiteSpace(giftId)) return BadRequest();
        var success = _store.RemoveGift(Id.Trim(), giftId);
        if (!success) return NotFound();
        return RedirectToPage("/Wishlist", new { id = Id });
    }

    public IActionResult OnGet()
    {
        if (string.IsNullOrWhiteSpace(Id)) return NotFound();
        if (!_store.TryGet(Id.Trim(), out var w)) return NotFound();
        Wishlist = w;
        return Page();
    }
}
