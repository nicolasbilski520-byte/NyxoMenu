using NyxoMenu.Configuration;
using NyxoMenu.Host;
using NyxoMenu.MemoryAI;
using NyxoMenu.NonHost;
using NyxoMenu.Statistics;
using NyxoMenu.Storage;
using UnityEngine;

namespace NyxoMenu.UI;

public sealed class MenuBehaviour : MonoBehaviour
{
    public MenuBehaviour(IntPtr ptr) : base(ptr) { }
    private bool _visible = true;
    private Rect _window = new(90, 70, 760, 550);
    private int _tab;
    private Vector2 _scroll;
    private string _status = "Ready — planner and manual journal only.";
    private readonly HostPlanner _host = new();
    private readonly PersonalTools _personal = new();
    private RoundMemoryService _memory = null!;
    private StatisticsService _stats = null!;
    private RoundStore _store = null!;
    private readonly string[] _tabs = { "Dashboard", "Host", "Non-host", "Memory AI", "Statistics", "Settings" };
    private string _input = "";

    private void Start()
    {
        _store = new RoundStore();
        _memory = new RoundMemoryService(_store);
        _stats = new StatisticsService(_store);
        _personal.Load();
    }

    private void Update()
    {
        if (Input.GetKeyDown(PluginConfig.ToggleKey.Value)) _visible = !_visible;
    }

    private void OnGUI()
    {
        if (!_visible) return;
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Mathf.Clamp(PluginConfig.Scale.Value, .75f, 1.5f), Mathf.Clamp(PluginConfig.Scale.Value, .75f, 1.5f), 1));
        Theme.Apply();
        _window = GUILayout.Window(654210, _window, DrawWindow, "NYXOMENU   /   1.0", GUILayout.MinWidth(680), GUILayout.MinHeight(500));
        GUI.matrix = Matrix4x4.identity;
    }

    private void DrawWindow(int id)
    {
        GUILayout.BeginHorizontal();
        for (var i = 0; i < _tabs.Length; i++)
        {
            var old = GUI.backgroundColor;
            if (_tab == i) GUI.backgroundColor = Theme.Accent;
            if (GUILayout.Button(_tabs[i], GUILayout.Height(30))) _tab = i;
            GUI.backgroundColor = old;
        }
        if (GUILayout.Button("×", GUILayout.Width(34), GUILayout.Height(30))) _visible = false;
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        _scroll = GUILayout.BeginScrollView(_scroll);
        switch (_tab)
        {
            case 0: Dashboard(); break;
            case 1: HostTab(); break;
            case 2: NonHostTab(); break;
            case 3: MemoryTab(); break;
            case 4: StatisticsTab(); break;
            case 5: SettingsTab(); break;
        }
        GUILayout.EndScrollView();
        GUILayout.Space(5);
        GUILayout.Label(_status, GUILayout.Height(24));
        GUI.DragWindow(new Rect(0, 0, 10000, 25));
    }

    private void Dashboard()
    {
        GUILayout.Label("ROUND COMPANION", Header());
        GUILayout.Label("Plan, take personal notes, and review what you observed. NyxoMenu does not inspect or change game state.");
        GUILayout.Space(12);
        GUILayout.BeginHorizontal();
        Card("HOST PLANNER", "Draft settings, roles and rules offline.", 220);
        Card("PERSONAL JOURNAL", _memory.Current == null ? "No active memory." : "Round memory active — " + _memory.Current.Timeline.Count + " timeline entries.", 220);
        Card("SESSION STATS", _stats.RoundCount + " saved round memories.", 220);
        GUILayout.EndHorizontal();
        GUILayout.Space(14);
        GUILayout.Label("QUICK ACTIONS", Header());
        if (GUILayout.Button("Create a new round memory", GUILayout.Height(34))) { _memory.NewRound(); _status = "New local round memory created."; }
        GUILayout.Label("All player, lobby and event details are entered manually. Screenshot capture requires separate opt-in and an explicit click.", Muted());
    }

    private void HostTab()
    {
        GUILayout.Label("HOST / LOBBY PLANNER", Header());
        GUILayout.Label("Suggestions are notes only; they are never applied to a live lobby.", Muted());
        _host.Map = Field("Map", _host.Map);
        _host.PlayerCount = Field("Players", _host.PlayerCount);
        _host.Speed = Field("Speed", _host.Speed);
        _host.Cooldown = Field("Emergency cooldown", _host.Cooldown);
        _host.Discussion = Field("Discussion time", _host.Discussion);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save preset")) _status = _host.SavePreset() ? "Planner preset saved locally." : "Could not save planner preset.";
        if (GUILayout.Button("Load preset")) _status = _host.LoadPreset() ? "Planner preset loaded." : "No valid saved planner preset was found.";
        if (GUILayout.Button("Random suggestion")) { _host.Randomize(); _status = "Generated a draft suggestion."; }
        GUILayout.EndHorizontal();
        _host.Roles = Field("Role configuration notes", _host.Roles);
        _host.Rules = Field("Rules (one per line)", _host.Rules);
        GUILayout.Label("Preset: " + _host.PresetName);
    }

    private void NonHostTab()
    {
        GUILayout.Label("NON-HOST / PERSONAL TOOLS", Header());
        _personal.LobbyNotes = Field("Lobby settings observed (manual)", _personal.LobbyNotes);
        _personal.RoleGuide = Field("Role guide notes", _personal.RoleGuide);
        _personal.TaskReference = Field("Task reference", _personal.TaskReference);
        _personal.MeetingNotes = Field("Personal meeting notes", _personal.MeetingNotes);
        _personal.Preset = Field("Personal preset", _personal.Preset);
        if (GUILayout.Button("Save personal notes"))
        {
            try { _personal.Save(); _status = "Personal notes saved locally."; }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { _status = "Could not save personal notes."; }
        }
    }

    private void MemoryTab()
    {
        GUILayout.Label("MEMORY AI / ROUND JOURNAL", Header());
        GUILayout.Label("No game data is read automatically. Enter only information you are allowed to record.", Muted());
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("New round")) _memory.NewRound();
        if (GUILayout.Button("Pause / resume journal")) _memory.TogglePause();
        if (GUILayout.Button("Save summary")) { _memory.Save(); _status = "Round memory saved."; }
        if (GUILayout.Button("Delete all round data")) { _memory.DeleteAll(); _status = "Round memories and saved screenshots deleted."; }
        GUILayout.EndHorizontal();
        if (_memory.Current == null)
        {
            GUILayout.Label("No active round. Create one to begin.");
            return;
        }
        GUILayout.Label(_memory.Paused ? "JOURNAL PAUSED" : "JOURNAL ACTIVE", Header());
        _input = Field("Add observation, statement, or reported event", _input);
        if (GUILayout.Button("Add to timeline")) { _memory.AddObservation(_input); _input = ""; }
        GUILayout.Label("Round summary / question (local evidence only)");
        var query = GUILayout.TextField(_memory.Query, GUILayout.MinHeight(60));
        _memory.Query = query;
        if (GUILayout.Button("Summarize / answer from notes")) _memory.AnswerFromNotes();
        GUILayout.TextArea(_memory.Answer, GUILayout.MinHeight(60));
        GUILayout.Label("Timeline", Header());
        foreach (var item in _memory.Current.Timeline.TakeLast(30)) GUILayout.Label($"[{item.Time:HH:mm:ss}] {item.Text}");
        GUILayout.Space(10);
        GUILayout.Label("SCREENSHOT ANALYSIS (optional; external service)", Header());
        var consent = GUILayout.Toggle(PluginConfig.ScreenshotConsent.Value, "I have appropriate permission; enable explicit capture");
        if (consent != PluginConfig.ScreenshotConsent.Value) PluginConfig.ScreenshotConsent.Value = consent;
        if (GUILayout.Button("Capture screenshot now (explicit action)"))
        {
            var path = _memory.CaptureScreenshot(PluginConfig.ScreenshotConsent.Value);
            _status = path == null ? "Capture blocked: opt-in required or journal unavailable." : "Screenshot requested: " + path;
        }
        if (GUILayout.Button("Analyze latest saved screenshot")) _memory.AnalyzeLatestScreenshot();
        if (GUILayout.Button("Delete saved screenshots")) _memory.DeleteScreenshots();
        GUILayout.Label(_memory.AiStatus, Muted());
        GUILayout.Label("AI output is an uncertain interpretation, not proof of hidden roles. Captures are saved locally and sent only after Analyze is clicked.", Muted());
    }

    private void StatisticsTab()
    {
        GUILayout.Label("STATISTICS", Header());
        GUILayout.Label("Saved round memories: " + _stats.RoundCount);
        GUILayout.Label("Timeline entries in active round: " + (_memory.Current?.Timeline.Count ?? 0));
        GUILayout.Label("Statistics are derived only from locally saved journal data.", Muted());
    }

    private void SettingsTab()
    {
        GUILayout.Label("SETTINGS & PRIVACY", Header());
        PluginConfig.Scale.Value = GUILayout.HorizontalSlider(PluginConfig.Scale.Value, .75f, 1.5f);
        GUILayout.Label($"Interface scale: {PluginConfig.Scale.Value:0.00}");
        GUILayout.Label("Toggle key is configured in BepInEx/config/com.nicolasbilski520byte.nyxomenu.cfg (restart may be needed).", Muted());
        GUILayout.Label("Vision service: OpenAI-compatible chat-completions endpoint; configure URL, model and API key in the BepInEx config file.", Muted());
        GUILayout.Label("The API key is plaintext on disk. Never configure a service you do not trust.", Muted());
        if (GUILayout.Button("Hide window")) _visible = false;
    }

    private static void Card(string title, string body, int width)
    {
        GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(width), GUILayout.Height(90));
        GUILayout.Label(title, Header()); GUILayout.Label(body, Muted());
        GUILayout.EndVertical();
    }
    private static string Field(string label, string value)
    {
        GUILayout.Label(label);
        return GUILayout.TextField(value ?? "", GUILayout.MinHeight(25));
    }
    private static GUIStyle Header() => new(GUI.skin.label) { fontSize = 17, fontStyle = FontStyle.Bold };
    private static GUIStyle Muted() => new(GUI.skin.label) { normal = { textColor = Theme.Muted }, wordWrap = true };
}
