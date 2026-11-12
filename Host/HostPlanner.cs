namespace NyxoMenu.Host;

internal sealed class HostPlanner
{
    private readonly Random _random = new();
    public string Map = "";
    public string PlayerCount = "10";
    public string Speed = "1.0x";
    public string Cooldown = "25s";
    public string Discussion = "30s";
    public string Roles = "";
    public string Rules = "";
    public string PresetName = "Draft";

    public void SavePreset()
    {
        var data = string.Join("\n", new[] { Map, PlayerCount, Speed, Cooldown, Discussion, Roles, Rules });
        File.WriteAllText(PresetPath, data);
    }
    public void LoadPreset()
    {
        if (!File.Exists(PresetPath)) return;
        var d = File.ReadAllLines(PresetPath);
        if (d.Length > 0) Map = d[0]; if (d.Length > 1) PlayerCount = d[1]; if (d.Length > 2) Speed = d[2];
        if (d.Length > 3) Cooldown = d[3]; if (d.Length > 4) Discussion = d[4]; if (d.Length > 5) Roles = d[5];
        if (d.Length > 6) Rules = string.Join("\n", d.Skip(6));
        PresetName = "Saved draft";
    }
    public void Randomize()
    {
        PlayerCount = _random.Next(6, 16).ToString();
        Speed = _random.Next(8, 16) / 10f + "x";
        Cooldown = _random.Next(10, 46) + "s";
        Discussion = _random.Next(15, 61) + "s";
        PresetName = "Random draft suggestion";
    }
    private static string PresetPath => Path.Combine(BepInEx.Paths.ConfigPath, "NyxoMenu", "host-preset.txt");
}
