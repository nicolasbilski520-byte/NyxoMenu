# Changelog

## [1.0.0] - 2026-11-13 (release preparation; not yet released)

### Added
- Local persistence and startup loading for manually maintained personal notes.
- JSON host-planner presets that retain multiline role notes and rules, plus safe handling for missing or invalid presets and compatibility loading for the earlier line-based preset file.
- Documentation of the implemented planning, journal, statistics, and optional screenshot-analysis features and their limitations.

### Fixed
- Public-field serialization for personal notes using `System.Text.Json` with `IncludeFields`.
- Creation of configuration directories before writing personal notes or host presets.
- Duplicate screenshot-consent control; capture now uses the single persisted consent setting and remains blocked unless enabled.
- Host preset save/load UI feedback for failed or unavailable preset files.

### Notes
- This is release preparation only. A successful DLL build and in-game test have not been performed or claimed. Do not publish a release until the matching BepInEx 6 IL2CPP build and in-game checks succeed.
- NyxoMenu remains a manual companion tool; it does not read hidden game state, automate gameplay, or apply planner settings to a live lobby.
