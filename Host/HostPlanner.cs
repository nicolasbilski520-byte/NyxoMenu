using System.Globalization;
using System.Text.Json;
using BepInEx;

namespace NyxoMenu.Host;

internal sealed class HostPlanner
{
    private readonly Random _random = new();
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
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
            File.WriteAllText(path, JsonSerializer.Serialize(new HostPreset
            {
                Map = Map,
                PlayerCount = PlayerCount,
                Speed = Speed,
                Cooldown = Cooldown,
                Discussion = Discussion,
                Roles = Roles,
                Rules = Rules
            }, JsonOptions));
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException or System.Security.SecurityException)
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
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException or NotSupportedException or System.Security.SecurityException)
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
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Security.SecurityException)
        {
            return false;
        }
    }

    public void Randomize()
    {
        PlayerCount = _random.Next(6, 16).ToString(CultureInfo.InvariantCulture);
        Speed = (_random.Next(8, 16) / 10f).ToString("0.0", CultureInfo.InvariantCulture) + "x";
        Cooldown = _random.Next(10, 46).ToString(CultureInfo.InvariantCulture) + "s";
        Discussion = _random.Next(15, 61).ToString(CultureInfo.InvariantCulture) + "s";
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

    private static string PresetPath => Path.Combine(BepInEx.Paths.ConfigPath, "NyxoMenu", "host-preset.json");
    private static string LegacyPresetPath => Path.Combine(BepInEx.Paths.ConfigPath, "NyxoMenu", "host-preset.txt");
}

public sealed class HostPreset
{
    public string Map { get; set; } = "";
    public string PlayerCount { get; set; } = "10";
    public string Speed { get; set; } = "1.0x";
    public string Cooldown { get; set; } = "25s";
    public string Discussion { get; set; } = "30s";
    public string Roles { get; set; } = "";
    public string Rules { get; set; } = "";

    public HostPreset() { }
}
