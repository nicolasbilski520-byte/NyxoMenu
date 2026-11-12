using BepInEx;
using BepInEx.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using NyxoMenu.Configuration;

namespace NyxoMenu.ScreenshotAnalysis;

/// <summary>OpenAI-compatible vision chat client. Sends an image only when Analyze is explicitly clicked.</summary>
internal sealed class VisionClient
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(60) };
    public async Task<string> AnalyzeAsync(string imagePath)
    {
        var endpoint = PluginConfig.VisionEndpoint.Value.Trim();
        var model = PluginConfig.VisionModel.Value.Trim();
        var key = PluginConfig.VisionApiKey.Value;
        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var uri) || (uri.Scheme != "https" && uri.Scheme != "http"))
            throw new InvalidOperationException("Configure a valid vision chat-completions endpoint in BepInEx config.");
        if (string.IsNullOrWhiteSpace(model) || string.IsNullOrWhiteSpace(key)) throw new InvalidOperationException("Configure a vision model and API key in BepInEx config.");
        var bytes = await File.ReadAllBytesAsync(imagePath);
        var data = Convert.ToBase64String(bytes);
        var payload = new
        {
            model,
            messages = new[] { new { role = "user", content = new object[] {
                new { type = "text", text = "Describe only visible information in this screenshot. Clearly distinguish direct visual observations from uncertainty. Do not infer or claim hidden roles, identity, or intent. This analysis may be wrong." },
                new { type = "image_url", image_url = new { url = "data:image/png;base64," + data } }
            } } },
            max_tokens = 500
        };
        using var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", key);
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        using var response = await Http.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode) throw new HttpRequestException($"Vision service returned {(int)response.StatusCode}: {body}");
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "Service returned empty content.";
    }
}
