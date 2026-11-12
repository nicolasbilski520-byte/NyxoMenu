namespace NyxoMenu.MemoryAI;

internal sealed class RoundMemory
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public DateTime StartedAt { get; set; } = DateTime.Now;
    public List<TimelineEntry> Timeline { get; set; } = new();
    public List<string> ScreenshotPaths { get; set; } = new();
}

internal sealed class TimelineEntry
{
    public DateTime Time { get; set; } = DateTime.Now;
    public string Text { get; set; } = "";
    public string Kind { get; set; } = "Manual observation";
    public bool IsAiInterpretation { get; set; }
}
