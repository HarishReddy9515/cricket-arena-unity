# Game Modes

Cricket Arena uses original game-mode systems rather than copying another cricket game's screens or rules.

## Modes

Quick Match:

- Six-ball chase
- Two wickets
- Balanced AI difficulty
- Fast entry point for testing gameplay

Practice Nets:

- Longer batting session
- Relaxed target
- High wicket allowance
- Useful for timing, controls, and animation testing

Career Chase:

- Level-based target scaling
- Skill points awarded after successful chases
- Designed for player progression and unlock systems

Tournament Chase:

- Round-based chase format
- Increasing target and AI difficulty per round
- Designed for cup/bracket expansion

Online Room:

- Uses the authoritative multiplayer server
- Keeps match rules aligned with online six-ball chase

## Unity Components

- `GameModeConfig` defines mode rules.
- `MatchManager.Configure` applies rules to the active match.
- `CareerProgressionManager` tracks level and skill points.
- `TournamentManager` tracks rounds and wins.
- `GameModeMenuController` exposes UI actions for each mode.

The scene builder creates mode buttons for Quick, Nets, Career, Cup, and Online.
