# Animation Pipeline

`PlayerAnimationDirector` is the gameplay-facing animation adapter. It lets batting, bowling, and match-state logic drive imported animation controllers through a stable parameter contract.

## Animator Parameters

Create these parameters in batter and bowler animator controllers:

```text
Bowl          Trigger
Hit           Trigger
Defend        Trigger
Wicket        Trigger
Celebrate     Trigger
ShotPower     Float
ShotIntent    Int
DeliverySpeed Float
MatchPhase    Int
```

## Shot Intent Values

```text
0 = Straight
1 = LoftLeft
2 = CutRight
3 = Defensive
```

## Match Phase Values

```text
0 = Menu
1 = WaitingForDelivery
2 = DeliveryLive
3 = ShotResolved
4 = InningsComplete
```

## Import Flow

1. Import legal rigged player models and animation clips.
2. Create batter and bowler animator controllers with the parameters above.
3. Assign the controllers to `CricketAssetManifest`.
4. Run `Cricket Arena > Validate Asset Readiness`.
5. Generate the scene with `Cricket Arena > Build Playable Prototype Scene`.

The animation director automatically applies manifest controllers when they are assigned.
