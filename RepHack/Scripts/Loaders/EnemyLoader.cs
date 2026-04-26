using RepHack;
using System.Text.Json;
using System.IO;

static class EnemyLoader
{
    static public List<EnemyData> Load()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Data", "enemies.json");
        string json = File.ReadAllText(path);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        try
        {
            List<EnemyData>? enemies = JsonSerializer.Deserialize<List<EnemyData>>(json, options);
            return enemies ?? new List<EnemyData>();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to load enemies.json: {e.Message}");
            return new List<EnemyData>();
        }
    }
}