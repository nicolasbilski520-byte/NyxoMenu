# Changelog

## [1.0.0] - 2026-11-13 (release preparation; not yet released)

### Fixed
- Added public DTO types with public parameterless constructors for .NET 6 `System.Text.Json` deserialization of personal notes and host presets, with safe handling for unsupported or malformed JSON.
- Added matching Unity interop references and build-time checks for `UnityEngine.IMGUIModule`, `UnityEngine.InputLegacyModule`, and `UnityEngine.ScreenCaptureModule` in the installed BepInEx interop directory.
- Added multiline editors for host role/rule text and personal notes while keeping short planner fields single-line.
- Restricted vision services to HTTPS, with an explicit loopback-only HTTP development exception that refuses API keys.
- Prevented overlapping screenshot analysis and ensured analysis output is persisted to the round that initiated the request.
- Added guarded journal save/deletion operations and UI feedback for storage failures.
- Formatted randomized numeric suggestions with invariant culture.
- Updated documentation to describe the actual behavior and limitations.

### Notes
- This is release preparation only. A successful DLL build and in-game test have not been performed or claimed. Do not publish a release until the matching BepInEx 6 IL2CPP build and in-game checks succeed.
- NyxoMenu remains a manual companion tool; it does not read hidden game state, automate gameplay, or apply planner settings to a live lobby.
