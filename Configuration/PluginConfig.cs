using BepInEx.Configuration;
using UnityEngine;

namespace NyxoMenu.Configuration;

internal static class PluginConfig
{
    public const string Guid = "com.nicolasbilski520byte.nyxomenu";
    public static ConfigEntry<float> Scale { get; private set; } = null!;
    public static ConfigEntry<KeyCode> ToggleKey { get; private set; } = null!;
    public static ConfigEntry<bool> ScreenshotConsent { get; private set; } = null!;
    public static ConfigEntry<string> VisionEndpoint { get; private set; } = null!;
    public static ConfigEntry<string> VisionModel { get; private set; } = null!;
    public static ConfigEntry<string> VisionApiKey { get; private set; } = null!;

    public static void Initialize(ConfigFile config)
    {
        Scale = config.Bind("Interface", "Scale", 1f, "UI scale (0.75–1.5).");
        ToggleKey = config.Bind("Interface", "ToggleKey", KeyCode.F8, "Toggle the NyxoMenu window.");
        ScreenshotConsent = config.Bind("Privacy", "AllowScreenshotCapture", false, "Explicit opt-in required before screenshot capture.");
        VisionEndpoint = config.Bind("Vision", "ChatCompletionsEndpoint", "", "OpenAI-compatible vision chat-completions URL; blank disables analysis.");
        VisionModel = config.Bind("Vision", "Model", "", "Vision-capable model name from your configured service.");
        VisionApiKey = config.Bind("Vision", "ApiKey", "", "Service credential (stored in plaintext in the local config file).");
    }
}
