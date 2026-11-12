using NyxoMenu.ScreenshotAnalysis;
using NyxoMenu.Storage;
using UnityEngine;

namespace NyxoMenu.MemoryAI;

internal sealed class RoundMemoryService
{
    private readonly RoundStore _store;
    private readonly VisionClient _vision = new();
    public RoundMemory? Current { get; private set; }
    public bool Paused { get; private set; }
    public string Query = "";
    public string Answer = "";
    public string AiStatus = "Vision analysis disabled until explicitly configured.";
    public RoundMemoryService(RoundStore store) => _store = store;
    public void NewRound() { if (Current != null) _store.Save(Current); Current = new RoundMemory(); Paused = false; _store.Save(Current); }
    public void TogglePause() => Paused = !Paused;
    public void AddObservation(string text)
    {
        if (Current == null || Paused || string.IsNullOrWhiteSpace(text)) return;
        Current.Timeline.Add(new TimelineEntry { Text = text.Trim() }); _store.Save(Current);
    }
    public void Save() { if (Current != null) _store.Save(Current); }
    public void DeleteAll() { Current = null; _store.DeleteAll(); }
    public string? CaptureScreenshot(bool consent)
    {
        if (!consent || Current == null || Paused) return null;
        var dir = _store.ScreenshotDirectory;
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, $"{DateTime.Now:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}.png");
        ScreenCapture.CaptureScreenshot(path);
        Current.ScreenshotPaths.Add(path); _store.Save(Current);
        return path;
    }
    public async void AnalyzeLatestScreenshot()
    {
        var path = Current?.ScreenshotPaths.LastOrDefault();
        if (path == null) { AiStatus = "No screenshot captured for this round."; return; }
        try
        {
            AiStatus = "Waiting for screenshot file, then submitting to configured vision service…";
            for (var i = 0; i < 40 && !File.Exists(path); i++) await Task.Delay(250);
            if (!File.Exists(path)) { AiStatus = "Screenshot file was not created."; return; }
            var result = await _vision.AnalyzeAsync(path);
            if (Current != null) { Current.Timeline.Add(new TimelineEntry { Text = "AI interpretation (uncertain): " + result, Kind = "AI interpretation", IsAiInterpretation = true }); _store.Save(Current); }
            AiStatus = result;
        }
        catch (Exception ex) { AiStatus = "Analysis failed: " + ex.Message; }
    }
    public void DeleteScreenshots()
    {
        if (Current == null) return;
        foreach (var path in Current.ScreenshotPaths) try { if (File.Exists(path)) File.Delete(path); } catch { }
        Current.ScreenshotPaths.Clear(); _store.Save(Current); AiStatus = "Saved screenshots deleted.";
    }
    public void AnswerFromNotes()
    {
        if (Current == null) { Answer = "Create a round memory first."; return; }
        var entries = Current.Timeline;
        if (entries.Count == 0) { Answer = "No observations have been recorded. Add manual notes before asking for a summary."; return; }
        Answer = "Local-notes summary (not an independent verification): " + string.Join(" | ", entries.Select(e => e.Text).TakeLast(12));
        Answer += "\nFor contradictions, compare attributed statements and timestamps manually. These notes cannot establish hidden roles.";
    }
}
