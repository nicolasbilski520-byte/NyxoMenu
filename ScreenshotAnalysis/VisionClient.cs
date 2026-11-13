using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using NyxoMenu.Configuration;

namespace NyxoMenu.ScreenshotAnalysis;

/// <summary>OpenAI-compatible vision client. Sends an image only when Analyze is explicitly clicked.</summary>
internal sealed class VisionClient
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(60) };

    public async Task<string> AnalyzeAsync(string imagePath)
    {
        var endpoint = PluginConfig.VisionEndpoint.Value.Trim();
        var model = PluginConfig.VisionModel.Value.Trim();
        var key = PluginConfig.VisionApiKey.Value;
        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp))
            throw new InvalidOperationException("Configure a valid HTTPS vision endpoint. HTTP is permitted only for a localhost development endpoint without an API key.");

        var isLocalHttp = uri.Scheme == Uri.UriSchemeHttp && uri.IsLoopback;
        if (uri.Scheme == Uri.UriSchemeHttp && !isLocalHttp)
            throw new InvalidOperationException("Vision endpoints must use HTTPS. Ordinary HTTP is not permitted.");
        if (isLocalHttp && !string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("An API key cannot be sent to a localhost endpoint over HTTP. Clear the key for local development or use HTTPS.");
        if (string.IsNullOrWhiteSpace(model))
            throw new InvalidOperationException("Configure a vision model in BepInEx config.");
        if (!isLocalHttp && string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Configure an API key for the HTTPS vision service in BepInEx config.");

        var bytes = await File.ReadAllBytesAsync(imagePath);
        var data = Convert.ToBase64String(bytes);
        var payload = new
        {
            model,
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = new object[]
                    {
                        new { type = "text", text = "Describe only visible information in this screenshot. Clearly distinguish direct visual observations from uncertainty. Do not infer or claim hidden roles, identities, or game state that cannot be seen in the image." },
                        new { type = "image_url", image_url = new { url = "data:image/png;base64," + data } }
                    }
                }
            },
            max_tokens = 500
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, uri);
        if (!string.IsNullOrWhiteSpace(key)) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", key);
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        using var response = await Http.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode) throw new HttpRequestException($"Vision service returned {(int)response.StatusCode}: {body}");
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "Service returned empty content.";
    }
}
