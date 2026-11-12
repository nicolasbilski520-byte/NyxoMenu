# Build / smoke test checklist

1. Install the .NET 6 SDK and the BepInEx 6 IL2CPP distribution matching the target game's installed build.
2. Verify the four reference assemblies listed in `NyxoMenu.csproj` exist beneath `BepInExDir`; do not substitute Mono assemblies for IL2CPP.
3. Run `dotnet build NyxoMenu.csproj -c Release -p:BepInExDir="<game>/BepInEx"`.
4. Install the output DLL in `BepInEx/plugins`, launch the game, and inspect `BepInEx/LogOutput.log` for plugin-load or Unity API compatibility issues.
5. Test dashboard visibility, settings tabs, saving/deleting notes and round JSON, screenshot opt-in, capture, and service failure behavior in a private test session.

Automated Unity integration tests are not included: they require the target game's proprietary assemblies and a running Unity game. The project includes build-time checks for key reference paths, but compilation and in-game validation must be performed locally against the intended game version. Do not enable screenshot service transmission until its endpoint and privacy policy have been reviewed.
