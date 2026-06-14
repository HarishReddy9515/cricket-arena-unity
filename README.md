# Cricket Arena Unity

Cricket Arena Unity is the real game-engine foundation for an original premium mobile cricket game. It is designed for Unity, Android/iOS, realistic 3D assets, animation-driven batting/bowling, haptics, and online multiplayer.

This is **not** a clone of Real Cricket, WCC, EA Sports FC, or PUBG. It is an original cricket game architecture targeting a premium mobile sports-game feel.

## What This Provides

- Unity-ready folder structure
- C# gameplay scripts
- batting timing controller
- bowling delivery controller
- ball physics controller
- adaptive AI bowling strategy
- batting timing assist and feedback
- match state manager
- original game modes: quick match, practice nets, career, tournament, online room
- premium mobile lobby UI scaffold
- 3D lobby camera and showcase player staging
- loadout customization for bat, kit, and boost
- inventory, season missions, and graphics preset controls
- local save/load for loadout, season, and graphics preferences
- procedural stadium banners and crowd color bands
- cinematic camera director
- mobile haptics bridge
- WebSocket-style multiplayer client scaffold
- asset pipeline folders for legal models, animations, audio, UI, and VFX
- production asset manifest and runtime placeholder replacement
- gameplay-driven animation parameter contract
- Android APK/AAB build automation
- Docker-ready authoritative multiplayer server
- local and GitHub smoke checks
- room cleanup, capacity limits, latency telemetry, and mobile quality presets
- Android/mobile build checklist

## Required Unity Setup

Install:

- Unity Hub
- Unity 2022 LTS or Unity 6 LTS
- Android Build Support
- iOS Build Support if targeting iPhone

Recommended Unity packages:

- Universal Render Pipeline
- Cinemachine
- Input System
- Shader Graph
- Netcode for GameObjects or Mirror
- Addressables

## Asset Requirements

Put legal assets here:

```text
Assets/Art/Models/Stadium/
Assets/Art/Models/Players/
Assets/Art/Animations/Batting/
Assets/Art/Animations/Bowling/
Assets/Audio/
Assets/VFX/
Assets/UI/
```

Do not use ripped game assets, EA assets, PUBG assets, Real Cricket assets, WCC assets, IPL/ICC/team logos, or copyrighted player likenesses.

## First Unity Steps

1. Create a new Unity 3D URP project.
2. Copy this repo's `Assets` and `ProjectSettings` folders into the Unity project.
3. Install Cinemachine and Input System.
4. In Unity, run `Cricket Arena > Create Recommended Asset Folders`.
5. Run `Cricket Arena > Create Asset Manifest`.
6. Assign legal prefabs/audio/materials into `Assets/Art/CricketAssetManifest.asset`.
7. Run `Cricket Arena > Validate Asset Readiness`.
8. Run `Cricket Arena > Configure Android Mobile Build`.
9. Run `Cricket Arena > Build Playable Prototype Scene`.
10. Save the generated scene as `Assets/Scenes/ArenaPrototype.unity`.
11. Press Play.

The scene builder creates:

- procedural oval stadium
- pitch
- floodlights
- placeholder batter and bowler
- physics ball
- camera rigs
- match manager
- batting and bowling controllers
- score HUD
- mobile controls scaffold
- replay recorder
- impact VFX hooks

## Unity Menu Tools

- `Cricket Arena > Create Recommended Asset Folders`
- `Cricket Arena > Create Asset Manifest`
- `Cricket Arena > Validate Asset Readiness`
- `Cricket Arena > Configure Android Mobile Build`
- `Cricket Arena > Build Playable Prototype Scene`
- `Cricket Arena > Build Android APK`
- `Cricket Arena > Build Android AAB`

## Build And Release

See [docs/BUILD_AND_RELEASE.md](docs/BUILD_AND_RELEASE.md). The project includes Unity batch-mode Android build entry points, local smoke checks, GitHub Actions checks, and Docker deployment files for the authoritative server.

## Project Status

See [docs/PROJECT_STATUS.md](docs/PROJECT_STATUS.md) for the current implemented scope, Unity requirements, asset requirements, and remaining commercial-game gaps.

## Multiplayer Operations

See [docs/MULTIPLAYER_OPERATIONS.md](docs/MULTIPLAYER_OPERATIONS.md). The server includes room-code sanitization, player limits, stale room cleanup, and a two-client protocol smoke test.

## Mobile Performance

See [docs/MOBILE_PERFORMANCE.md](docs/MOBILE_PERFORMANCE.md). Runtime quality presets tune frame rate, anti-aliasing, shadow distance, and LOD bias for battery, balanced, and performance modes.

## Authoritative Server

The `server/` folder contains a dependency-free Node.js authoritative match server scaffold.

Run:

```bash
cd server
node authoritative-server.js
```

Health check:

```text
http://localhost:8790/health
```

Protocol events:

- `join_room`
- `ready`
- `request_delivery`
- `delivery`
- `shot`
- `match_state`
- `ping` / `pong`

The server owns delivery selection and shot outcome resolution so clients cannot freely invent results.

## Asset Pipeline

See [docs/ASSET_PIPELINE.md](docs/ASSET_PIPELINE.md). The project uses `CricketAssetManifest` and `RuntimeAssetBinder` so legal production models, animation controllers, materials, and audio can replace procedural placeholders cleanly.

## Animation Pipeline

See [docs/ANIMATION_PIPELINE.md](docs/ANIMATION_PIPELINE.md). `PlayerAnimationDirector` maps gameplay events to animator triggers and parameters for bowling, batting, wickets, celebrations, shot power, delivery speed, and match phase.

## Game Modes

See [docs/GAME_MODES.md](docs/GAME_MODES.md). The generated prototype includes buttons for Quick Match, Practice Nets, Career Chase, Tournament Chase, and Online Room. These are original systems built around configurable match rules and progression.

## Gameplay AI

See [docs/GAMEPLAY_AI.md](docs/GAMEPLAY_AI.md). Offline bowling now adapts to mode difficulty, runs required, balls remaining, and wickets lost. Batting has light timing assist, timing feedback, and season mission rewards.

## UI Direction

See [docs/UI_DIRECTION.md](docs/UI_DIRECTION.md). The scene builder now creates an original premium mobile sports lobby with top status bar, mode panel, squad/loadout panel, bottom action bar, highlighted primary play action, 3D showcase player, stadium atmosphere dressing, and lobby-to-gameplay screen switching.

## Unity Multiplayer Flow

The generated prototype scene now includes a `RealtimeMatchClient` and `MultiplayerLobbyController`.

Scene controls:

- Connect to the match server
- Join a room code
- Mark player ready
- Request the next authoritative delivery
- Send shot timing and intent back to the server

Run the server first:

```bash
cd server
npm start
```

Then open the generated Unity scene, press **Connect**, enter or keep `ARENA-24`, press **Join**, and press **Ready**. `RealtimeMatchClient` uses `ClientWebSocket` on supported editor, desktop, and mobile targets. WebGL still needs a browser WebSocket plugin because Unity does not expose the same .NET socket path there.

`NetworkGameplaySynchronizer` connects server events to gameplay:

- Server `delivery` events launch the Unity ball with the authoritative speed, swing, bounce, and difficulty.
- Server `match_state` events sync score, wickets, balls, match status, and outcome message.
- Offline mode still works through local bowling and batting controls when no server is connected.

## Target Features

- Real 3D stadium
- Rigged player models
- animation-driven batting and bowling
- realistic ball physics
- adaptive AI bowling
- batting timing assist and feedback
- mobile haptics
- cinematic replay camera
- online friend rooms
- quick match, practice nets, career, tournament, and online room modes
- premium mobile sports lobby UI
- 3D lobby camera and showcase player staging
- loadout customization and stadium atmosphere dressing
- inventory, season missions, and graphics preset controls
- local save/load and mission reward wallet loop
- authoritative server validation
- optimized mobile rendering
- replay highlights
- impact VFX and stadium audio hooks
- production asset manifest and runtime placeholder replacement
- gameplay-driven animation parameter contract
- Android APK/AAB build automation
- Docker-ready authoritative multiplayer server
- room cleanup, latency telemetry, and mobile quality presets

## Current Status

This repo is a Unity-ready engineering foundation with an Editor scene generator. It does not include AAA copyrighted assets. Visual quality depends on the legal assets imported into Unity.
