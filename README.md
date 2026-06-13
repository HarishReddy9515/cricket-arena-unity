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
4. In Unity, run `Cricket Arena > Build Playable Prototype Scene`.
5. Save the generated scene as `Assets/Scenes/ArenaPrototype.unity`.
6. Press Play.

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

## Current Status

This repo is a Unity-ready engineering foundation with an Editor scene generator. It does not include AAA copyrighted assets. Visual quality depends on the legal assets imported into Unity.
