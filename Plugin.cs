using BepInEx;
using BepInEx.Unity.IL2CPP;
using Il2CppInterop.Runtime.Injection;
using NyxoMenu.Configuration;
using NyxoMenu.UI;

namespace NyxoMenu;

[BepInPlugin(PluginConfig.Guid, "NyxoMenu", "1.0.0")]
public sealed class Plugin : BasePlugin
{
    public override void Load()
    {
        PluginConfig.Initialize(Config);
        ClassInjector.RegisterTypeInIl2Cpp<MenuBehaviour>();
        AddComponent<MenuBehaviour>();
        Log.LogInfo("NyxoMenu 1.0 loaded. No game state is read or modified.");
    }
}
