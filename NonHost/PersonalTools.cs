using System.Text.Json;
using BepInEx;

namespace NyxoMenu.NonHost;

internal sealed class PersonalTools
{
    private const string FileName = "personal-notes.json";
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        IncludeFields = true,
        WriteIndented = true
    };

    public string LobbyNotes = "";
    public string RoleGuide = "";
    public string TaskReference = "";
    public string MeetingNotes = "";
    public string Preset = "";

    public bool Load()
    {
        var path = GetPath();
        if (!File.Exists(path)) return false;

        try
        {
            var saved = JsonSerializer.Deserialize<PersonalNotesData>(File.ReadAllText(path), JsonOptions);
            if (saved == null) return false;

            LobbyNotes = saved.LobbyNotes ?? "";
            RoleGuide = saved.RoleGuide ?? "";
            TaskReference = saved.TaskReference ?? "";
            MeetingNotes = saved.MeetingNotes ?? "";
            Preset = saved.Preset ?? "";
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException or NotSupportedException or System.Security.SecurityException)
        {
            return false;
        }
    }

    public void Save()
    {
        var path = GetPath();
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var data = new PersonalNotesData
        {
            LobbyNotes = LobbyNotes,
            RoleGuide = RoleGuide,
            TaskReference = TaskReference,
            MeetingNotes = MeetingNotes,
            Preset = Preset
        };
        File.WriteAllText(path, JsonSerializer.Serialize(data, JsonOptions));
    }

    private static string GetPath() => Path.Combine(Paths.ConfigPath, "NyxoMenu", FileName);
}

public sealed class PersonalNotesData
{
    public string LobbyNotes { get; set; } = "";
    public string RoleGuide { get; set; } = "";
    public string TaskReference { get; set; } = "";
    public string MeetingNotes { get; set; } = "";
    public string Preset { get; set; } = "";

    public PersonalNotesData() { }
}
