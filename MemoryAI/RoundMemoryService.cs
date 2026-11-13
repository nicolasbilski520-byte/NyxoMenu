using NyxoMenu.ScreenshotAnalysis;
using NyxoMenu.Storage;
using UnityEngine;

namespace NyxoMenu.MemoryAI;

internal sealed class RoundMemoryService
{
    private readonly RoundStore _store;
    private readonly VisionClient _vision = new();
    private int _analysisInProgress;

    public RoundMemory? Current { get; private set; }
    public bool Paused { get; private set; }
    public bool AnalysisInProgress => Volatile.Read(ref _analysisInProgress) != 0;
    public string Query = "";
    public string Answer = "";
    public string AiStatus = "Vision analysis disabled until explicitly configured.";

    public RoundMemoryService(RoundStore store) => _store = store;

    public bool NewRound()
    {
        var round = new RoundMemory();
        Current = round;
        Paused = false;
        return TrySave(round);
    }

    public void TogglePause() => Paused = !Paused;

    public bool AddObservation(string text)
    {
        var round = Current;
        if (round == null || Paused || string.IsNullOrWhiteSpace(text)) return false;
        round.Timeline.Add(new TimelineEntry { Text = text.Trim() });
        return TrySave(round);
    }

    public bool Save() => Current != null && TrySave(Current);

    public bool DeleteAll()
    {
        try
        {
            _store.DeleteAll();
            Current = null;
            return true;
        }
        catch (Exception ex) when (IsStorageException(ex))
        {
            return false;
        }
    }

    public string? CaptureScreenshot(bool consent)
    {
        var round = Current;
        if (!consent || round == null || Paused) return null;
        try
        {
            var dir = _store.ScreenshotDirectory;
            Directory.CreateDirectory(dir);
            var path = Path.Combine(dir, $"{DateTime.Now:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}.png");
            ScreenCapture.CaptureScreenshot(path);
            round.ScreenshotPaths.Add(path);
            if (!TrySave(round)) return null;
            return path;
        }
        catch (Exception ex) when (IsStorageException(ex))
        {
            return null;
        }
    }

    public async void AnalyzeLatestScreenshot()
    {
        if (Interlocked.CompareExchange(ref _analysisInProgress, 1, 0) != 0)
        {
            AiStatus = "Screenshot analysis is already in progress.";
            return;
        }

        var requestedRound = Current;
        var path = requestedRound?.ScreenshotPaths.LastOrDefault();
        if (requestedRound == null || path == null)
        {
            AiStatus = "No screenshot captured for this round.";
            Volatile.Write(ref _analysisInProgress, 0);
            return;
        }

        try
        {
            if (ReferenceEquals(Current, requestedRound)) AiStatus = "Waiting for screenshot file, then submitting to configured vision service…";
            for (var i = 0; i < 40 && !File.Exists(path); i++) await Task.Delay(250);
            if (!File.Exists(path))
            {
                if (ReferenceEquals(Current, requestedRound)) AiStatus = "Screenshot file was not created.";
                return;
            }

            var result = await _vision.AnalyzeAsync(path);
            requestedRound.Timeline.Add(new TimelineEntry
            {
                Text = "AI interpretation (uncertain): " + result,
                Kind = "AI interpretation",
                IsAiInterpretation = true
            });
            var saved = TrySave(requestedRound);
            if (ReferenceEquals(Current, requestedRound))
                AiStatus = saved ? result : "Analysis completed, but its journal entry could not be saved.";
        }
        catch (Exception ex)
        {
            if (ReferenceEquals(Current, requestedRound)) AiStatus = "Analysis failed: " + ex.Message;
        }
        finally
        {
            Volatile.Write(ref _analysisInProgress, 0);
        }
    }

    public void DeleteScreenshots()
    {
        var round = Current;
        if (round == null) return;
        try
        {
            foreach (var path in round.ScreenshotPaths)
                if (File.Exists(path)) File.Delete(path);
            round.ScreenshotPaths.Clear();
            if (TrySave(round)) AiStatus = "Saved screenshots deleted.";
            else AiStatus = "Screenshots deleted, but the journal update could not be saved.";
        }
        catch (Exception ex) when (IsStorageException(ex))
        {
            AiStatus = "Could not delete saved screenshots: " + ex.Message;
        }
    }

    public void AnswerFromNotes()
    {
        if (Current == null) { Answer = "Create a round memory first."; return; }
        var entries = Current.Timeline;
        if (entries.Count == 0) { Answer = "No observations have been recorded. Add manual notes before asking for a summary."; return; }
        Answer = "Local-notes summary (not an independent verification): " + string.Join(" | ", entries.Select(e => e.Text).TakeLast(12));
        Answer += "\nFor contradictions, compare attributed statements and timestamps manually. These notes cannot establish hidden roles.";
    }

    private bool TrySave(RoundMemory round)
    {
        try
        {
            _store.Save(round);
            return true;
        }
        catch (Exception ex) when (IsStorageException(ex))
        {
            return false;
        }
    }

    private static bool IsStorageException(Exception ex) =>
        ex is IOException or UnauthorizedAccessException or System.Security.SecurityException or NotSupportedException or System.Text.Json.JsonException;
}
