using UnityEngine;

namespace NyxoMenu.UI;

internal static class Theme
{
    public static readonly Color Background = new(0.075f, 0.055f, 0.12f, 0.98f);
    public static readonly Color Panel = new(0.14f, 0.10f, 0.21f, 1f);
    public static readonly Color Accent = new(0.62f, 0.39f, 0.96f, 1f);
    public static readonly Color Text = new(0.93f, 0.90f, 0.98f, 1f);
    public static readonly Color Muted = new(0.68f, 0.63f, 0.74f, 1f);

    public static void Apply()
    {
        GUI.color = Text;
        GUI.backgroundColor = Panel;
        GUI.contentColor = Text;
    }
}
