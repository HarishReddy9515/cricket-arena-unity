# Build and Release

This project is ready for Unity-based Android builds once Unity and Android Build Support are installed locally or in CI.

## Local Checks

```powershell
.\scripts\run-checks.ps1
```

## Generate Playable Scene

In Unity:

1. `Cricket Arena > Create Recommended Asset Folders`
2. `Cricket Arena > Create Asset Manifest`
3. Assign legal assets in `Assets/Art/CricketAssetManifest.asset`
4. `Cricket Arena > Build Playable Prototype Scene`
5. Save the generated scene as `Assets/Scenes/ArenaPrototype.unity`

## Android Build From Unity Menu

Use:

- `Cricket Arena > Configure Android Mobile Build`
- `Cricket Arena > Build Android APK`
- `Cricket Arena > Build Android AAB`

## Android Build From Command Line

Example:

```powershell
Unity.exe -batchmode -quit -projectPath . -executeMethod CricketArena.EditorTools.AndroidBuildPipeline.BuildAndroidFromCommandLine -outputPath Builds/Android/CricketArena.apk
```

For Play Store bundle:

```powershell
Unity.exe -batchmode -quit -projectPath . -executeMethod CricketArena.EditorTools.AndroidBuildPipeline.BuildAndroidFromCommandLine -aab -outputPath Builds/Android/CricketArena.aab
```

## Multiplayer Server

Run locally:

```bash
cd server
npm start
```

Run with Docker:

```bash
cd server
docker compose up --build
```

Health check:

```text
http://localhost:8790/health
```

## Release Gate

Before calling a release ready:

- Smoke checks pass
- Unity project opens without compile errors
- `ArenaPrototype.unity` is generated and saved
- Legal production assets are assigned in the manifest
- Android APK or AAB builds successfully
- Two clients can join the same server room
- Delivery, shot, and score sync are verified against the authoritative server
