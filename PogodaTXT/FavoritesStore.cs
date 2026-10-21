using System.Text.Encodings.Web;
using System.Text.Json;

namespace PogodaTXT;

/// <summary>
/// Lista ulubionych miast zapisywana w pliku JSON w %APPDATA%\PogodaTXT.
/// </summary>
public sealed class FavoritesStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        // Polskie znaki zapisujemy wprost, żeby plik dało się czytać i edytować ręcznie
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private readonly string _path;
    private readonly List<Location> _items;

    public FavoritesStore(string? path = null)
    {
        _path = path ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PogodaTXT", "ulubione.json");
        _items = Load(_path);
    }

    public IReadOnlyList<Location> Items => _items;

    public bool Contains(Location location) => _items.Any(f => IsSamePlace(f, location));

    public void Add(Location location)
    {
        if (Contains(location))
            return;
        _items.Add(location);
        Save();
    }

    public void Remove(Location location)
    {
        if (_items.RemoveAll(f => IsSamePlace(f, location)) > 0)
            Save();
    }

    // Porównujemy po współrzędnych, bo nazwa i region mogą się różnić zależnie od języka zapytania
    private static bool IsSamePlace(Location a, Location b) =>
        Math.Abs(a.Latitude - b.Latitude) < 0.001 && Math.Abs(a.Longitude - b.Longitude) < 0.001;

    private static List<Location> Load(string path)
    {
        try
        {
            return File.Exists(path)
                ? JsonSerializer.Deserialize<List<Location>>(File.ReadAllText(path)) ?? []
                : [];
        }
        catch (Exception ex) when (ex is JsonException or IOException or UnauthorizedAccessException)
        {
            // Uszkodzony lub niedostępny plik nie powinien blokować programu
            return [];
        }
    }

    private void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        File.WriteAllText(_path, JsonSerializer.Serialize(_items, JsonOptions));
    }
}
