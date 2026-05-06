namespace RepHack;
using System.Text.Json;
using System.IO;

static class ItemLoader
{
    static public List<ItemData> Load()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Data", "items.json");
        string json = File.ReadAllText(path);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        try
        {
            List<ItemData>? items = JsonSerializer.Deserialize<List<ItemData>>(json, options);
            return items ?? new List<ItemData>();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to load items.json: {e.Message}");
            return new List<ItemData>();
        }
    }
}