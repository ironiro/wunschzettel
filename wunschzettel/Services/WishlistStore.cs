using System.Collections.Concurrent;
using System.Text.Json;
using wunschzettel.Models;

namespace wunschzettel.Services;

/// <summary>
/// Simple file-backed store for wishlists. Not intended for high concurrency or multi-instance use.
/// Generates unique 4-digit numeric IDs for wishlists.
/// </summary>
public class WishlistStore
{
    private readonly string _filePath;
    private readonly object _lock = new object();
    private readonly Dictionary<string, Wishlist> _map = new Dictionary<string, Wishlist>();

    public WishlistStore(IWebHostEnvironment env)
    {
        var dataDir = Path.Combine(env.ContentRootPath, "data");
        Directory.CreateDirectory(dataDir);
        _filePath = Path.Combine(dataDir, "wishlists.json");
        Load();
    }

    private void Load()
    {
        lock (_lock)
        {
            if (!File.Exists(_filePath)) return;
            try
            {
                var text = File.ReadAllText(_filePath);
                var list = JsonSerializer.Deserialize<List<Wishlist>>(text, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Wishlist>();
                _map.Clear();
                foreach (var w in list)
                {
                    if (!string.IsNullOrWhiteSpace(w.Id)) _map[w.Id] = w;
                }
            }
            catch { /* ignore malformed file */ }
        }
    }

    private void Save()
    {
        lock (_lock)
        {
            var list = _map.Values.ToList();
            var text = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, text);
        }
    }

    public Wishlist Create(string? title = null)
    {
        lock (_lock)
        {
            string id;
            var rand = new Random();
            do
            {
                id = rand.Next(1000, 10000).ToString();
            } while (_map.ContainsKey(id));

            var w = new Wishlist { Id = id, Title = title };
            _map[id] = w;
            Save();
            return w;
        }
    }

    public bool TryGet(string id, out Wishlist? wishlist)
    {
        lock (_lock)
        {
            return _map.TryGetValue(id, out wishlist);
        }
    }

    public IEnumerable<Wishlist> ListAll()
    {
        lock (_lock) return _map.Values.ToList();
    }

    public bool AddGift(string wishlistId, Gift gift)
    {
        lock (_lock)
        {
            if (!_map.TryGetValue(wishlistId, out var w)) return false;
            w.Gifts ??= new List<Gift>();
            w.Gifts.Add(gift);
            Save();
            return true;
        }
    }

    public bool RemoveGift(string wishlistId, string giftId)
    {
        lock (_lock)
        {
            if (!_map.TryGetValue(wishlistId, out var w) || w.Gifts == null) return false;
            var removed = w.Gifts.RemoveAll(g => string.Equals(g.Id, giftId, StringComparison.OrdinalIgnoreCase)) > 0;
            if (removed) Save();
            return removed;
        }
    }

    public bool ToggleGiftBought(string wishlistId, string giftId)
    {
        lock (_lock)
        {
            if (!_map.TryGetValue(wishlistId, out var w) || w.Gifts == null) return false;
            var g = w.Gifts.FirstOrDefault(x => string.Equals(x.Id, giftId, StringComparison.OrdinalIgnoreCase));
            if (g == null) return false;
            g.IsBought = !g.IsBought;
            if (!g.IsBought) g.BoughtBy = null;
            Save();
            return true;
        }
    }

    public bool SetGiftBought(string wishlistId, string giftId, string? boughtBy)
    {
        lock (_lock)
        {
            if (!_map.TryGetValue(wishlistId, out var w) || w.Gifts == null) return false;
            var g = w.Gifts.FirstOrDefault(x => string.Equals(x.Id, giftId, StringComparison.OrdinalIgnoreCase));
            if (g == null) return false;
            g.IsBought = true;
            g.BoughtBy = string.IsNullOrWhiteSpace(boughtBy) ? null : boughtBy.Trim();
            Save();
            return true;
        }
    }

    public bool UnsetGiftBought(string wishlistId, string giftId)
    {
        lock (_lock)
        {
            if (!_map.TryGetValue(wishlistId, out var w) || w.Gifts == null) return false;
            var g = w.Gifts.FirstOrDefault(x => string.Equals(x.Id, giftId, StringComparison.OrdinalIgnoreCase));
            if (g == null) return false;
            g.IsBought = false;
            g.BoughtBy = null;
            Save();
            return true;
        }
    }
}
