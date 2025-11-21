using System.Collections.Generic;

namespace wunschzettel.Models;

public class Wishlist
{
    // 4-digit numeric ID stored as string (e.g. "1234")
    public string Id { get; set; } = string.Empty;

    // Title or name of the wishlist
    public string? Title { get; set; }

    // List of gifts - attributes will be added later
    public List<Gift> Gifts { get; set; } = new List<Gift>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
