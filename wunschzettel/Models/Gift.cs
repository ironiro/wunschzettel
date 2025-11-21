namespace wunschzettel.Models;

public class Gift
{
    // Unique id for the gift (internal)
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    public string? Name { get; set; }
    public string? Note { get; set; }
    public decimal? Price { get; set; }
    // Optional link to a shop or product page
    public string? Link { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsBought { get; set; } = false;
    // Name of the person who bought the gift (optional)
    public string? BoughtBy { get; set; }
}
