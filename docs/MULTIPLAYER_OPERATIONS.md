# Multiplayer Operations

The authoritative Node server owns room state, delivery selection, and shot outcome resolution.

## Room Rules

- Room codes are sanitized to uppercase letters, numbers, and hyphens.
- Room codes are capped at 16 characters.
- Default room is `ARENA-24`.
- `MAX_ROOM_PLAYERS` defaults to `2`.
- `ROOM_TTL_MS` defaults to 10 minutes.
- Stale rooms are cleaned up automatically when the standalone server is running.
- Clients are rate limited per message window to reduce spam and accidental duplicate input.

## Environment Variables

```text
PORT=8790
MAX_ROOM_PLAYERS=2
ROOM_TTL_MS=600000
RATE_LIMIT_WINDOW_MS=1000
RATE_LIMIT_MAX_MESSAGES=24
```

## Server Endpoints

```text
GET /health
GET /rooms
GET /metrics
```

`/metrics` exposes uptime, active clients, active rooms, total connections, messages, rejected messages, deliveries, shots, and completed rooms.

## Client Telemetry

Unity `RealtimeMatchClient` tracks:

- `PlayerId`
- `LastLatencyMs`
- server error messages
- last inbound and outbound JSON payloads for debugging
- reconnect attempts

The client sends heartbeat pings and can reconnect with backoff when the connection drops unexpectedly.

## Validation

Run:

```powershell
.\scripts\run-checks.ps1
```

The protocol smoke test starts the server and validates a two-client join, ready, delivery, shot, and score-sync flow.
