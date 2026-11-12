# NyxoMenu

NyxoMenu is a client-side, privacy-first Among Us companion plugin for BepInEx 6 IL2CPP. It provides planning/reference tools and a manually maintained round journal; it does not read game internals, reveal roles, automate gameplay, or interact with anti-cheat systems.

## Compatibility and build prerequisites

The repository metadata does not identify an installed game build, and Among Us updates can change Unity and loader versions. The project is therefore configured for the BepInEx 6 IL2CPP plugin model and uses the game/loader assemblies from a local installation rather than guessing package versions. The current Among Us PC release is commonly reported as Unity 2022.3 / IL2CPP, but verify your installed build and BepInEx IL2CPP distribution before building. This is not a claim that every game or loader release is compatible.

Requirements: Windows, .NET 6 SDK, an Among Us installation, and a BepInEx 6 IL2CPP installation for the same game build. Set `BepInExDir` to the installed `BepInEx` directory. The project expects `core/BepInEx.Core.dll`, `core/BepInEx.Unity.IL2CPP.dll`, `core/Il2CppInterop.Runtime.dll`, and `interop/UnityEngine.CoreModule.dll` under that directory. Adjust only if your distribution lays these assemblies out differently.

```powershell
dotnet build .\NyxoMenu.csproj -c Release -p:BepInExDir="C:\Games\Among Us\BepInEx"
```

The output is `bin/Release/net6.0/NyxoMenu.dll`. Copy it to the game's `BepInEx/plugins/` directory. Check the BepInEx log for load errors. This repository does not include proprietary Unity/game assemblies or a prebuilt DLL; build against the exact local installation. No compile is claimed here because those assemblies and a build runner are not available in this repository-editing environment.

## Features

- Dark-purple dashboard with Dashboard, Host, Non-host, Memory AI, Statistics, and Settings tabs; configurable scale and toggle key.
- Host planning: lobby settings, editable presets, role-plan notes, rule list, and randomized *suggestions* (never applied to a live lobby).
- Non-host references: manually entered lobby notes, role guide, task reference, meeting notes, and personal presets.
- Memory AI: user-created round journal, timeline/events, manually entered statements and player notes, summaries and evidence-limited tips. Data is explicitly entered by the user; no hidden role/state access exists.
- Screenshot analysis is opt-in and disabled until a vision-compatible OpenAI chat-completions endpoint, model, and API key are configured. The user must check consent and click capture, then separately request analysis. Images and memories can be deleted locally.
- Local JSON storage under `BepInEx/config/NyxoMenu/`; screenshots are stored separately and removable from the UI.

## Privacy and limitations

Screenshots may contain other players' identifying information. Obtain appropriate consent before capturing or sending an image. NyxoMenu never captures in the background. API credentials are stored in the local BepInEx config file in plaintext; use a key intended for this purpose and protect that file. Image analysis transmits the selected screenshot to the endpoint you configured. Only use a trusted service and review its privacy/retention terms.

This is a companion/planning tool, not a live game integration. Lobby/player data must be entered by the user; presets and random settings are suggestions. Vision models can be wrong. Their responses are saved as uncertain AI interpretations and must never be treated as proof of hidden roles. Screenshot capture timing and Unity assembly locations vary by game build. Multiplayer, anti-cheat, role inspection, automatic lobby control, automatic screenshot capture, and game-data integration are intentionally not implemented.

## Project layout

See `NyxoMenu.csproj`. Modules are separated into `Configuration/`, `UI/`, `Host/`, `NonHost/`, `MemoryAI/`, `ScreenshotAnalysis/`, `Storage/`, and `Statistics/`. `Plugin.cs` is the BepInEx entry point.
