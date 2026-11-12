using BepInEx;
using NyxoMenu.MemoryAI;
using System.Text.Json;

namespace NyxoMenu.Storage;

internal sealed class RoundStore
{
    private readonly string _roundDir = Path.Combine(Paths.ConfigPath, "NyxoMenu", "rounds");
    public string ScreenshotDirectory => Path.Combine(Paths.ConfigPath, "NyxoMenu", "screenshots");
    public RoundStore() => Directory.CreateDirectory(_roundDir);
    public void Save(RoundMemory round)
    {
        Directory.CreateDirectory(_roundDir);
        File.WriteAllText(Path.Combine(_roundDir, round.Id + ".json"), JsonSerializer.Serialize(round, new JsonSerializerOptions { WriteIndented = true }));
    }
    public int Count => Directory.Exists(_roundDir) ? Directory.GetFiles(_roundDir, "*.json").Length : 0;
    public void DeleteAll()
    {
        if (Directory.Exists(_roundDir)) foreach (var file in Directory.GetFiles(_roundDir, "*.json")) File.Delete(file);
        if (Directory.Exists(ScreenshotDirectory)) foreach (var file in Directory.GetFiles(ScreenshotDirectory)) File.Delete(file);
    }
}
