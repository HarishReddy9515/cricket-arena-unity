# Mobile Performance

`MobilePerformanceManager` applies runtime quality presets for Android and iOS targets.

## Presets

Battery:

- 30 FPS target
- anti-aliasing off
- shorter shadow distance
- lower LOD bias

Balanced:

- 60 FPS target
- 2x anti-aliasing
- mid shadow distance
- standard LOD bias

Performance:

- 60 FPS target
- 2x anti-aliasing
- longer shadow distance
- higher LOD bias

## Android Build Defaults

`Cricket Arena > Configure Android Mobile Build` sets:

- IL2CPP
- ARM64
- landscape orientation
- Android API 23 minimum
- ASTC texture target
- 60 FPS target
- balanced quality defaults

## Next Optimization Pass

Once real assets are imported, profile on a physical Android device and tune:

- mesh LODs
- texture resolution and ASTC compression level
- animation rig complexity
- shadow caster count
- stadium crowd density
- particle overdraw
