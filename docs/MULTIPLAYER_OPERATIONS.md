# Multiplayer Operations

The authoritative Node server owns room state, delivery selection, and shot outcome resolution.

## Room Rules

- Room codes are sanitized to uppercase letters, numbers, and hyphens.
- Room codes are capped at 16 characters.
- Default room is `ARENA-24`.
- `MAX_ROOM_PLAYERS` defaults to `2`.
- `ROOM_TTL_MS` defaults to 10 minutes.
- Stale rooms are cleaned up automatically when the standalone server is running.

## Environment Variables

```text
PORT=8790
MAX_ROOM_PLAYERS=2
ROOM_TTL_MS=600000
```

## Client Telemetry

Unity `RealtimeMatchClient` tracks:

- `PlayerId`
- `LastLatencyMs`
- server error messages
- last inbound and outbound JSON payloads for debugging

## Validation

Run:

```powershell
.\scripts\run-checks.ps1
```

The protocol smoke test starts the server and validates a two-client join, ready, delivery, shot, and score-sync flow.
