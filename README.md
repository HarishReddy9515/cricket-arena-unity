# Cricket Arena Unity

Cricket Arena Unity is the real game-engine foundation for an original premium mobile cricket game. It is designed for Unity, Android/iOS, realistic 3D assets, animation-driven batting/bowling, haptics, and online multiplayer.

This is **not** a clone of Real Cricket, WCC, EA Sports FC, or PUBG. It is an original cricket game architecture targeting a premium mobile sports-game feel.

## What This Provides

- Unity-ready folder structure
- C# gameplay scripts
- batting timing controller
- bowling delivery controller
- ball physics controller
- match state manager
- cinematic camera director
- mobile haptics bridge
- WebSocket-style multiplayer client scaffold
- asset pipeline folders for legal models, animations, audio, UI, and VFX
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
5. Run `Cricket Arena > Validate Asset Readiness`.
6. Run `Cricket Arena > Configure Android Mobile Build`.
7. Run `Cricket Arena > Build Playable Prototype Scene`.
8. Save the generated scene as `Assets/Scenes/ArenaPrototype.unity`.
9. Press Play.

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
- `Cricket Arena > Validate Asset Readiness`
- `Cricket Arena > Configure Android Mobile Build`
- `Cricket Arena > Build Playable Prototype Scene`

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

## Unity Multiplayer Flow

The generated prototype scene now includes a `RealtimeMatchClient` and `MultiplayerLobbyController`.

Scene controls:

- Connect to the match server
- Join a room code
- Mark player ready
- Request the next authoritative delivery
- Send shot timing and intent back to the server

`RealtimeMatchClient` currently produces and parses server-compatible JSON. Attach a production WebSocket transport such as NativeWebSocket, Mirror, Netcode for GameObjects, or a platform transport plugin to send `LastOutboundJson` and pass received payloads into `ReceiveJson`.

## Target Features

- Real 3D stadium
- Rigged player models
- animation-driven batting and bowling
- realistic ball physics
- mobile haptics
- cinematic replay camera
- online friend rooms
- authoritative server validation
- optimized mobile rendering
- replay highlights
- impact VFX and stadium audio hooks

## Current Status

This repo is a Unity-ready engineering foundation with an Editor scene generator. It does not include AAA copyrighted assets. Visual quality depends on the legal assets imported into Unity.
