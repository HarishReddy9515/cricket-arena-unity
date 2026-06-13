# Asset Pipeline

Cricket Arena Unity is built to use original, licensed assets. Do not import ripped files from existing games, sports broadcasts, league packs, or copyrighted player likenesses.

## Required Core Assets

Place production assets in these folders:

```text
Assets/Art/Models/Stadium/
Assets/Art/Models/Players/
Assets/Art/Animations/Batting/
Assets/Art/Animations/Bowling/
Assets/Audio/
Assets/VFX/
Assets/UI/
```

Run these Unity menu actions:

1. `Cricket Arena > Create Recommended Asset Folders`
2. `Cricket Arena > Create Asset Manifest`
3. Assign legal prefabs, animation controllers, clips, materials, and audio to `Assets/Art/CricketAssetManifest.asset`
4. `Cricket Arena > Validate Asset Readiness`
5. `Cricket Arena > Build Playable Prototype Scene`

## Manifest Slots

`CricketAssetManifest` contains slots for:

- Stadium prefab
- Batter prefab
- Bowler prefab
- Bat prefab
- Wicket prefab
- Batter and bowler animator controllers
- Batting and bowling animation clips
- Bat-hit, crowd, wicket, and boundary audio
- Grass, pitch, ball, and kit materials

## Runtime Binding

`RuntimeAssetBinder` replaces procedural placeholders at scene start when a manifest is assigned. This lets the prototype run with placeholders while still supporting production-grade assets once imported.
