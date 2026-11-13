using System.Text.Json;
using BepInEx;

namespace NyxoMenu.Host;

internal sealed class HostPlanner
{
    private readonly Random _random = new();
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        IncludeFields = true,
        WriteIndented = true
    };

    public string Map = "";
    public string PlayerCount = "10";
    public string Speed = "1.0x";
    public string Cooldown = "25s";
    public string Discussion = "30s";
    public string Roles = "";
    public string Rules = "";
    public string PresetName = "Draft";

    public bool SavePreset()
    {
        try
        {
            var path = PresetPath;
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, JsonSerializer.Serialize(this, JsonOptions));
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return false;
        }
    }

    public bool LoadPreset()
    {
        if (File.Exists(PresetPath))
        {
            try
            {
                var saved = JsonSerializer.Deserialize<HostPreset>(File.ReadAllText(PresetPath), JsonOptions);
                if (saved == null) return false;
                Apply(saved);
                PresetName = "Saved draft";
                return true;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
            {
                return false;
            }
        }

        // Read presets created by earlier builds, which stored one value per line.
        if (!File.Exists(LegacyPresetPath)) return false;
        try
        {
            var lines = File.ReadAllLines(LegacyPresetPath);
            if (lines.Length == 0) return false;
            if (lines.Length > 0) Map = lines[0];
            if (lines.Length > 1) PlayerCount = lines[1];
            if (lines.Length > 2) Speed = lines[2];
            if (lines.Length > 3) Cooldown = lines[3];
            if (lines.Length > 4) Discussion = lines[4];
            if (lines.Length > 5) Roles = lines[5];
            if (lines.Length > 6) Rules = string.Join("\n", lines.Skip(6));
            PresetName = "Saved draft (legacy)";
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return false;
        }
    }

    public void Randomize()
    {
        PlayerCount = _random.Next(6, 16).ToString();
        Speed = _random.Next(8, 16) / 10f + "x";
        Cooldown = _random.Next(10, 46) + "s";
        Discussion = _random.Next(15, 61) + "s";
        PresetName = "Random draft suggestion";
    }

    private void Apply(HostPreset saved)
    {
        Map = saved.Map ?? "";
        PlayerCount = saved.PlayerCount ?? "10";
        Speed = saved.Speed ?? "1.0x";
        Cooldown = saved.Cooldown ?? "25s";
        Discussion = saved.Discussion ?? "30s";
        Roles = saved.Roles ?? "";
        Rules = saved.Rules ?? "";
    }

    private sealed class HostPreset
    {
        public string Map = "";
        public string PlayerCount = "10";
        public string Speed = "1.0x";
        public string Cooldown = "25s";
        public string Discussion = "30s";
        public string Roles = "";
        public string Rules = "";
    }

    private static string PresetPath => Path.Combine(BepInEx.Paths.ConfigPath, "NyxoMenu", "host-preset.json");
    private static string LegacyPresetPath => Path.Combine(BepInEx.Paths.ConfigPath, "NyxoMenu", "host-preset.txt");
}
