using System.Text.Json;
using BepInEx;

namespace NyxoMenu.NonHost;

internal sealed class PersonalTools
{
    private const string FileName = "personal-notes.json";
    public string LobbyNotes = "";
    public string RoleGuide = "";
    public string TaskReference = "";
    public string MeetingNotes = "";
    public string Preset = "";

    public void Save()
    {
        var path = Path.Combine(Paths.ConfigPath, "NyxoMenu", FileName);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
    }
}
