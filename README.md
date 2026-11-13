# NyxoMenu

NyxoMenu is a client-side Among Us companion plugin for BepInEx 6 IL2CPP. It provides offline planning and manually maintained reference/journal tools. It does not read or modify live game state.

## Compatibility and build

The project targets .NET 6 and references BepInEx 6 IL2CPP and Unity interop assemblies supplied by the installed game. Use a BepInEx IL2CPP distribution compatible with the target game's installed build; do not substitute Mono assemblies. The project validates the expected assemblies beneath the BepInEx `core/` and `interop/` directories, including the Unity IMGUI, legacy input, and screen-capture modules used by this code.

Requirements: Windows, the .NET 6 SDK, an Among Us installation, and its matching BepInEx 6 IL2CPP installation. Set `BepInExDir` to the installed `BepInEx` directory:

```powershell
dotnet build .\NyxoMenu.csproj -c Release -p:BepInExDir="C:\Games\Among Us\BepInEx"
```

The expected output is `bin/Release/net6.0/NyxoMenu.dll`. Copy it to the game's `BepInEx/plugins/` directory only after a successful build. This repository does not include proprietary Unity/game assemblies, so a build cannot be confirmed without the matching local installation. No in-game test or successful DLL build is claimed here. See [BUILDING.md](BUILDING.md) for a manual build and test checklist.

## Implemented features

- Dark-purple dashboard with Dashboard, Host, Non-host, Memory AI, Statistics, and Settings tabs; configurable UI scale and toggle key.
- Host planner for manually drafted map/player/speed/timer settings, multiline role and rule notes, JSON preset save/load, and randomized settings suggestions. These are drafts only and are never applied to a lobby.
- Personal notes for manually recorded lobby settings, role guide, task reference, meeting notes, and personal presets. Notes are multiline-editable, stored locally, and loaded on plugin startup.
- Manual round journal with timeline entries, pause/resume, local summaries, saved-round counts, and deletion of saved round data. Storage failures are reported in the UI rather than escaping journal actions.
- Optional screenshot capture, gated by explicit consent and a separate capture button. Sending a saved image to an OpenAI-compatible vision endpoint requires a separate Analyze action and configured endpoint/model. Requests do not overlap and results are written to the originating round.
- Vision endpoints require HTTPS. HTTP is allowed only for a loopback/local development endpoint (for example `http://localhost:port`) with no API key; credentials are never sent over HTTP.
- Local JSON storage under `BepInEx/config/NyxoMenu/`; screenshot images are stored separately and can be deleted from the UI.

## Privacy, safety, and limitations

NyxoMenu does not inspect hidden roles, game internals, player state, or lobby state; all notes and observations must be entered manually. It does not automate gameplay or change live game settings. Random host settings are suggestions. Vision output is an uncertain interpretation of visible screenshot content, not evidence of hidden roles.

Screenshot capture is disabled unless the user enables the consent toggle, and a capture still requires an explicit button click. Screenshots may contain identifying information; obtain appropriate permission before capturing. Image analysis sends the selected saved screenshot to the configured external service only after the user clicks Analyze. API keys are stored in plaintext in the local BepInEx configuration file and only transmitted over HTTPS.

The project has no automated Unity integration tests. Compatibility and runtime behavior depend on the target game's and BepInEx installation versions; compile and test locally before distributing the DLL.

## Project layout

`Plugin.cs` is the BepInEx entry point. Source is organized in `Configuration/`, `UI/`, `Host/`, `NonHost/`, `MemoryAI/`, `ScreenshotAnalysis/`, `Storage/`, and `Statistics/`.
