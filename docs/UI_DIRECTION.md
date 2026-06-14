# UI Direction

Cricket Arena should feel like an original premium mobile sports game, not a copied interface from another product.

## Lobby Layout

The generated prototype scene uses:

- top status bar for score, mode, status, network state, and equation
- left mode panel for Quick Match, Practice Nets, Career, Tournament, and Online Room
- right squad panel for team identity, room code, connection, and loadout
- bottom action bar for ready, delivery, shot, and primary play action
- loadout controls for bat, kit, and boost selection
- a separate 3D lobby camera focused on the showcase player
- a rotating player pedestal for the pre-match lobby
- screen-state switching from lobby chrome into gameplay HUD

## Visual Style

- cinematic stadium-first screen
- dark translucent panels over the 3D scene
- gold primary action color
- blue secondary mode/network color
- compact mobile-friendly buttons
- no copied logos, layouts, icons, or brand language from existing games

## Unity Components

- `ArenaLobbySkin` applies the lobby color treatment and default lobby text.
- `LoadoutController` updates bat, kit, boost, XP, and coin display.
- `ArenaScreenDirector` switches lobby panels, gameplay panels, and camera view.
- `LobbyShowcaseController` rotates and floats the lobby showcase player.
- `ArenaSceneBuilder` creates the full lobby hierarchy during prototype scene generation.
- `GameModeMenuController` and `MultiplayerLobbyController` wire gameplay actions into the lobby.

## Stadium Atmosphere

The scene builder adds procedural banner strips and crowd color bands around the stadium so the generated scene has more visual density before licensed 3D crowd and signage assets are imported.
