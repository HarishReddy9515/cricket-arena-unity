# Gameplay AI

`AIBowlingStrategy` makes offline bowling react to match state instead of choosing deliveries randomly.

## Inputs

- selected game mode difficulty from `GameModeConfig`
- runs required
- balls remaining
- wickets lost
- delivery difficulty values

## Behavior

The strategy computes pressure from required run rate and wickets lost, then chooses the delivery whose difficulty is closest to the current target difficulty. A small randomness factor keeps bowling from becoming fully predictable.

## Mode Impact

- Practice Nets stays easier.
- Quick Match uses balanced bowling.
- Career and Tournament get harder as level or round increases.
- Online Room still uses the authoritative server delivery selection.
